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

        public async Task<Guid> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Charges)
                .Include(r => r.Payments)
                .Include(r => r.Invoices)
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId);

            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found.");

            if ((dto.Items?.Count ?? 0) == 0)
                throw new InvalidOperationException("No items selected for invoicing.");

            // Genero numero fattura
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

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                ReservationId = reservation.Id,
                InvoiceNumber = $"{yearPrefix}-{nextNumber:D4}",
                IssueDate = DateTime.UtcNow,
                Status = InvoiceStatus.Issued,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy ?? "System",
                Customer = new InvoiceCustomer
                {
                    FirstName = dto.Customer.FirstName,
                    LastName = dto.Customer.LastName,
                    TaxCode = dto.Customer.TaxCode,
                    Address = dto.Customer.Address,
                    City = dto.Customer.City,
                    Country = dto.Customer.Country
                },
                Items = new List<InvoiceItem>(),
                InvoicePayments = new List<InvoicePayment>()
            };

            decimal totalAmount = 0;
            foreach (var itemDto in dto.Items)
            {
                // Room
                if (itemDto.Description.Contains("Room"))
                {
                    if (reservation.Room == null)
                        throw new InvalidOperationException("Reservation has no room to invoice.");

                    if (reservation.IsRoomInvoiced)
                        throw new InvalidOperationException("La camera è già stata fatturata.");

                    const decimal vatRate = 22;
                    decimal vatAmount = Math.Round(itemDto.UnitPrice * vatRate / (100 + vatRate), 2);

                    invoice.Items.Add(new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = invoice.Id,
                        Description = itemDto.Description,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        TotalPrice = itemDto.UnitPrice * itemDto.Quantity,
                        VatRate = vatRate,
                        VatAmount = vatAmount
                    });

                    reservation.IsRoomInvoiced = true;
                }
                // Charges
                else
                {
                    var charge = reservation.Charges.FirstOrDefault(c => c.Description == itemDto.Description && !c.IsInvoiced);
                    if (charge == null)
                        throw new InvalidOperationException($"Charge '{itemDto.Description}' cannot be invoiced.");

                    decimal vatAmount = Math.Round(itemDto.UnitPrice * itemDto.VatRate / (100 + itemDto.VatRate), 2);

                    invoice.Items.Add(new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = invoice.Id,
                        Description = charge.Description,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        TotalPrice = itemDto.UnitPrice * itemDto.Quantity,
                        VatRate = itemDto.VatRate,
                        VatAmount = vatAmount
                    });

                    charge.IsInvoiced = true;
                }

                totalAmount += itemDto.UnitPrice * itemDto.Quantity;
            }

            invoice.SubTotal = Math.Round(invoice.Items.Sum(i => i.TotalPrice - i.VatAmount), 2);
            invoice.TaxAmount = Math.Round(invoice.Items.Sum(i => i.VatAmount), 2);
            invoice.TotalAmount = Math.Round(invoice.SubTotal + invoice.TaxAmount, 2);

            // Apply available payments
            decimal remainingToPay = invoice.TotalAmount;

            foreach (var payment in reservation.Payments
                         .Where(p => p.Status == PaymentStatus.Pending && !p.IsInvoiced))
            {
                if (remainingToPay <= 0)
                    break;

                decimal applied = Math.Min(payment.Amount, remainingToPay);

                invoice.InvoicePayments.Add(new InvoicePayment
                {
                    InvoiceId = invoice.Id,
                    PaymentId = payment.Id,
                    AmountApplied = applied,
                    CreatedAt = DateTime.UtcNow
                });

                remainingToPay -= applied;

                // Brand payment as used
                payment.Status = PaymentStatus.Completed;
                payment.IsInvoiced = true;
            }

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice.Id;
        }


    } 
}
