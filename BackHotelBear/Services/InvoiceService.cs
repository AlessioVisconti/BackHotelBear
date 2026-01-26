using BackHotelBear.Models.Data;
using BackHotelBear.Models.Dtos.InvoiceDtos;
using BackHotelBear.Models.Entity.InvoiceAndEnum;
using BackHotelBear.Models.Entity.PaymentAndEnum;
using BackHotelBear.Models.Entity.ReservationAndEnum;
using BackHotelBear.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackHotelBear.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly HotelBearDbContext _context;
        public InvoiceService(HotelBearDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> CreateAndIssueInvoiceFromReservationAsync(Guid reservationId, InvoiceCustomerDto customer)
        {
            //Recupero i dati della prenotazione
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Charges)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found.");

            string yearPrefix = DateTime.UtcNow.Year.ToString();
            var lastInvoice = await _context.Invoices
                .Where(i => i.IssueDate.Year == DateTime.UtcNow.Year)
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastInvoice != null)
            {
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int last))
                    nextNumber = last + 1;
            }
            //creo la fattura e setto lo stato già ad Issued
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                ReservationId = reservationId,
                InvoiceNumber = $"{yearPrefix}-{nextNumber:D4}",
                IssueDate = DateTime.UtcNow,
                Status = InvoiceStatus.Issued,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",

                Items = new List<InvoiceItem>(),
                InvoicePayments = new List<InvoicePayment>(),
                Customer = new InvoiceCustomer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    TaxCode = customer.TaxCode,
                    Address = customer.Address,
                    City = customer.City,
                    Country = customer.Country
                }
            };
            //calcolo i vari totali
            decimal totalRoom = reservation.Room?.PriceForNight ?? 0;
            decimal totalCharges = reservation.Charges.Sum(c => c.Amount);
            decimal totalReservation = totalRoom + totalCharges;

            decimal alreadyInvoiced = await _context.InvoicePayments
                .Where(ip => ip.Invoice.ReservationId == reservationId)
                .SumAsync(ip => ip.AmountApplied);

            decimal totalPaid = reservation.Payments.Sum(p => p.Amount);
            decimal amountToInvoice = Math.Max(0, totalPaid - alreadyInvoiced);

            if (amountToInvoice <= 0)
                throw new InvalidOperationException("No available payment to create invoice.");

            decimal remaining = amountToInvoice;
            //setto il vat rate della stanza a 22
            if (reservation.Room != null && remaining > 0)
            {
                decimal roomAlreadyInvoiced = invoice.Items
                    .Where(i => i.Description.Contains("Room"))
                    .Sum(i => i.TotalPrice);

                decimal roomRemaining = totalRoom - roomAlreadyInvoiced;
                decimal roomToInvoice = Math.Min(roomRemaining, remaining);

                if (roomToInvoice > 0)
                {
                    const decimal vatRate = 22;
                    decimal vatAmount = Math.Round(roomToInvoice * vatRate / (100 + vatRate), 2);

                    invoice.Items.Add(new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        Description = $"Room {reservation.Room.RoomNumber}",
                        Quantity = 1,
                        UnitPrice = roomToInvoice,
                        TotalPrice = roomToInvoice,
                        VatRate = vatRate,
                        VatAmount = vatAmount
                    });

                    remaining -= roomToInvoice;
                }
            }

            foreach (var charge in reservation.Charges)
            {
                if (remaining <= 0) break;

                decimal chargeAlreadyInvoiced = invoice.Items
                    .Where(i => i.Description == charge.Description)
                    .Sum(i => i.TotalPrice);

                decimal chargeRemaining = charge.Amount - chargeAlreadyInvoiced;
                if (chargeRemaining <= 0) continue;

                decimal toInvoice = Math.Min(chargeRemaining, remaining);
                decimal vatAmount = Math.Round(toInvoice * charge.VatRate / (100 + charge.VatRate), 2);

                invoice.Items.Add(new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    Description = charge.Description,
                    Quantity = 1,
                    UnitPrice = toInvoice,
                    TotalPrice = toInvoice,
                    VatRate = charge.VatRate,
                    VatAmount = vatAmount
                });
                charge.IsInvoiced = true;
                remaining -= toInvoice;
            }

            invoice.SubTotal = Math.Round(invoice.Items.Sum(i => i.TotalPrice - i.VatAmount), 2);
            invoice.TaxAmount = Math.Round(invoice.Items.Sum(i => i.VatAmount), 2);
            invoice.TotalAmount = Math.Round(invoice.SubTotal + invoice.TaxAmount, 2);

            decimal invoiceRemaining = invoice.TotalAmount;
            //qui cambio lo status se Pending a Completed
            foreach (var payment in reservation.Payments)
            {
                if (invoiceRemaining <= 0) break;
                decimal alreadyUsed = await _context.InvoicePayments
                    .Where(ip => ip.PaymentId == payment.Id)
                    .SumAsync(ip => ip.AmountApplied);

                decimal available = payment.Amount - alreadyUsed;
                if (available <= 0) continue;

                decimal applied = Math.Min(available, invoiceRemaining);

                invoice.InvoicePayments.Add(new InvoicePayment
                {
                    InvoiceId = invoice.Id,
                    PaymentId = payment.Id,
                    AmountApplied = applied,
                    CreatedAt = payment.PaidAt ?? DateTime.UtcNow
                });
                if (payment.Status == PaymentStatus.Pending && applied > 0)
                    payment.Status = PaymentStatus.Completed;

                invoiceRemaining -= applied;
            }

            decimal alreadyInvoicedTotal = await _context.Invoices
                .Where(i => i.ReservationId == reservationId)
                .SelectMany(i => i.Items)
                .SumAsync(i => i.TotalPrice);

            decimal totalInvoicedIncludingThis = alreadyInvoicedTotal + invoice.TotalAmount;

            reservation.PaymentStatus =
                totalInvoicedIncludingThis >= totalReservation
                    ? ReservationPaymentStatus.Paid
                    : totalInvoicedIncludingThis > 0
                        ? ReservationPaymentStatus.PartiallyPaid
                        : ReservationPaymentStatus.NotPaid;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice.Id;
        }
        public async Task<bool> CancelInvoiceAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) throw new KeyNotFoundException("Invoice not found.");
            if (invoice.Status == InvoiceStatus.Cancelled) throw new InvalidOperationException("Invoice is already cancelled.");

            invoice.Status = InvoiceStatus.Cancelled;
            invoice.DeletedAt = DateTime.UtcNow;
            invoice.DeletedBy = "System";
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.InvoicePayments)
                .Include(i => i.Customer)
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Room)
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Charges)
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Invoices)
                        .ThenInclude(inv => inv.Items)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
                return null;

            var reservation = invoice.Reservation;

            decimal totalReservation =
                (reservation.Room?.PriceForNight ?? 0)
                + reservation.Charges.Sum(c => c.Amount);

            decimal totalInvoiced = reservation.Invoices
                .Where(inv => inv.Id != invoice.Id)
                .SelectMany(inv => inv.Items)
                .Sum(it => it.TotalPrice) + invoice.Items.Sum(it => it.TotalPrice);
            return new InvoiceDto
            {
                Id = invoice.Id,
                ReservationId = invoice.ReservationId,
                InvoiceNumber = invoice.InvoiceNumber,
                Status = invoice.Status,
                IssueDate = invoice.IssueDate,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,

                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt,
                DeletedAt = invoice.DeletedAt,

                CreatedBy = await ResolveUserAsync(invoice.CreatedBy),
                UpdatedBy = await ResolveUserAsync(invoice.UpdatedBy),
                DeletedBy = await ResolveUserAsync(invoice.DeletedBy),

                BalanceDue = invoice.TotalAmount - invoice.InvoicePayments.Sum(p => p.AmountApplied),
                RemainingAmount = Math.Round(totalReservation - totalInvoiced, 2),

                Customer = new InvoiceCustomerDto
                {
                    FirstName = invoice.Customer.FirstName,
                    LastName = invoice.Customer.LastName,
                    TaxCode = invoice.Customer.TaxCode,
                    Address = invoice.Customer.Address,
                    City = invoice.Customer.City,
                    Country = invoice.Customer.Country
                },

                Items = invoice.Items.Select(it => new InvoiceItemDto
                {
                    Id = it.Id,
                    Description = it.Description,
                    UnitPrice = it.UnitPrice,
                    Quantity = it.Quantity,
                    TotalPrice = it.TotalPrice,
                    VatRate = it.VatRate,
                    VatAmount = it.VatAmount
                }).ToList(),

                Payments = invoice.InvoicePayments.Select(p => new InvoicePaymentDto
                {
                    PaymentId = p.PaymentId,
                    AmountApplied = p.AmountApplied,
                    CreatedAt = p.CreatedAt
                }).ToList()
            };
        }
        private async Task<string?> ResolveUserAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync();

            return user == null ? userId : $"{user.FirstName} {user.LastName}";
        }
    } 
}
