using BackHotelBear.Models.Data;
using BackHotelBear.Models.Dtos.ChargeDtos;
using BackHotelBear.Models.Dtos.GuestDtos;
using BackHotelBear.Models.Dtos.InvoiceDtos;
using BackHotelBear.Models.Dtos.PaymentDtos;
using BackHotelBear.Models.Dtos.ReservationDtos;
using BackHotelBear.Models.Entity.PaymentAndEnum;
using BackHotelBear.Models.Entity.ReservationAndEnum;
using BackHotelBear.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackHotelBear.Services
{
    public class ReservationService : IReservationService
    {
        private readonly HotelBearDbContext _context;
        public ReservationService(HotelBearDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationResult> CreateReservationAsync(CreateReservationDto dto, string? customerId = null)
        {
            var checkIn = dto.CheckIn.Date;
            var checkOut = dto.CheckOut.Date;

            if (checkIn < DateTime.UtcNow.Date)
                return new ReservationResult { Success = false, ErrorMessage = "Check-in cannot be in the past." };
            if (checkIn >= checkOut)
                return new ReservationResult { Success = false, ErrorMessage = "Check-out must be after check-in." };

            var roomExists = await _context.Rooms.AnyAsync(r => r.Id == dto.RoomId);
            if (!roomExists)
                return new ReservationResult { Success = false, ErrorMessage = "The selected room does not exist." };

            var isOccupied = await _context.Reservations.AnyAsync(r =>
                r.RoomId == dto.RoomId && r.DeletedAt == null &&
                r.CheckIn < checkOut && r.CheckOut > checkIn);

            if (isOccupied)
                return new ReservationResult { Success = false, ErrorMessage = "The room is already booked for the selected dates." };

            var reservation = new Reservation
            {
                CustomerId = customerId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Email = dto.Email,
                RoomId = dto.RoomId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Note = dto.Note,
                Status = ReservationStatus.Pending,

            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return new ReservationResult
            {
                Success = true,
                Reservation = await GetReservationByIdAsync(reservation.Id)
            };
        }

        public async Task<ReservationResult> UpdateReservationAsync(Guid reservationId, UpdateReservationDto dto, string? modifiedByUserId)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId && r.DeletedAt == null);
            if (reservation == null)
                return new ReservationResult { Success = false, ErrorMessage = "Reservation not found." };

            var newCheckIn = dto.CheckIn?.Date ?? reservation.CheckIn;
            var newCheckOut = dto.CheckOut?.Date ?? reservation.CheckOut;
            var newRoomId = dto.RoomId ?? reservation.RoomId;

            if (newCheckIn < DateTime.UtcNow.Date)
                return new ReservationResult { Success = false, ErrorMessage = "Check-in cannot be in the past." };

            if (newCheckIn >= newCheckOut)
                return new ReservationResult { Success = false, ErrorMessage = "Check-out must be after check-in." };

            var isOccupied = await _context.Reservations.AnyAsync(r =>
                r.Id != reservationId &&
                r.RoomId == newRoomId &&
                r.DeletedAt == null &&
                r.CheckIn < newCheckOut &&
                r.CheckOut > newCheckIn);

            if (isOccupied)
                return new ReservationResult { Success = false, ErrorMessage = "The room is already booked for the selected dates." };

            reservation.FirstName = dto.FirstName ?? reservation.FirstName;
            reservation.LastName = dto.LastName ?? reservation.LastName;
            reservation.Phone = dto.Phone ?? reservation.Phone;
            reservation.Email = dto.Email ?? reservation.Email;
            reservation.RoomId = newRoomId;
            reservation.CheckIn = newCheckIn;
            reservation.CheckOut = newCheckOut;
            reservation.Note = dto.Note ?? reservation.Note;
            reservation.Status = dto.Status ?? reservation.Status;

            if (dto.Status.HasValue)
                reservation.Status = dto.Status.Value;


            await _context.SaveChangesAsync();

            return new ReservationResult
            {
                Success = true,
                Reservation = await GetReservationByIdAsync(reservation.Id)
            };
        }

        public async Task<ReservationResult> CancelReservationAsync(Guid reservationId, string? cancelledByUserId = null)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Charges)
                .Include(r => r.Invoices)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.DeletedAt == null);

            if (reservation == null)
                return new ReservationResult { Success = false, ErrorMessage = "Reservation not found." };

            // Control: if there is at least one charge invoiced or an invoice already issued, it cannot be cancelled
            bool hasInvoicedCharges = reservation.Charges.Any(c => c.IsInvoiced);
            bool hasInvoices = reservation.Invoices.Any();

            if (hasInvoicedCharges || hasInvoices)
                return new ReservationResult
                {
                    Success = false,
                    ErrorMessage = "Cannot cancel reservation: it contains invoiced charges or issued invoices."
                };

            reservation.Status = ReservationStatus.Cancelled;
            reservation.DeletedAt = DateTime.UtcNow;
            reservation.DeletedBy = cancelledByUserId;

            await _context.SaveChangesAsync();

            return new ReservationResult
            {
                Success = true,
                Reservation = await GetReservationByIdAsync(reservation.Id)
            };
        }
        /*I apologize if this method is currently large. 
         In a possible refactor, I'll repurpose it with internal methods that will contain the same logic, 
         but divided by section to make it even more organized.
         Furthermore, if you want to use this method for a real-world application in a very large hotel with 
         many spacious rooms and many guests, and/or want to use more or less the same logic to retrieve 
         all guests in a group even if they are in multiple rooms, consider lazy loading to avoid overly bloating the query.*/
        public async Task<ReservationDetailDto?> GetReservationByIdAsync(Guid reservationId)
        {
            // Load the reservation with relations
            var reservation = await _context.Reservations
                .Where(r => r.Id == reservationId && r.DeletedAt == null)
                .Include(r => r.Room)
                .Include(r => r.Payments.Where(p => p.DeletedAt == null))
                    .ThenInclude(p => p.PaymentMethod)
                .Include(r => r.Charges)
                .Include(r => r.Guests)
                .Include(r => r.Invoices)
                    .ThenInclude(i => i.Items)
                .Include(r => r.Invoices)
                    .ThenInclude(i => i.InvoicePayments)
                .Include(r => r.Invoices)
                    .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync();

            if (reservation == null) return null;

            // Calculate number of nights
            int numberOfNights = (reservation.CheckOut.Date - reservation.CheckIn.Date).Days;

            // Room only if NOT invoiced, multiplied by number of nights
            decimal roomAmount = reservation.IsRoomInvoiced
                ? 0
                : (reservation.Room?.PriceForNight ?? 0) * numberOfNights;

            // Charges only if NOT invoiced
            decimal chargesAmount = reservation.Charges
                .Where(c => !c.IsInvoiced)
                .Sum(c => c.Amount);

            // Total remaining
            decimal remainingAmount = Math.Round(roomAmount + chargesAmount, 2);

            // PaymentStatus based on remaining
            ReservationPaymentStatus paymentStatus =
                remainingAmount <= 0
                    ? ReservationPaymentStatus.Paid
                    : ReservationPaymentStatus.NotPaid;

            // Retrieve all involved userIds
            var userIds = reservation.Guests.SelectMany(g => new[] { g.CreatedBy, g.UpdatedBy, g.DeletedBy })
                .Concat(reservation.Payments.SelectMany(p => new[] { p.CreatedBy, p.UpdatedBy, p.DeletedBy }))
                .Concat(reservation.Charges.SelectMany(c => new[] { c.CreatedBy, c.UpdatedBy, c.DeletedBy }))
                .Concat(reservation.Invoices.SelectMany(i => new[] { i.CreatedBy, i.UpdatedBy, i.DeletedBy }))
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            // Load all users in memory
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}");

            // Synchronous helper to get user name
            string GetUserName(string? userId) =>
                string.IsNullOrEmpty(userId)
                    ? "System"
                    : users.GetValueOrDefault(userId, "System");

            // Map Guest DTOs
            var guestDtos = reservation.Guests.Select(g => new GuestDto
            {
                Id = g.Id,
                FirstName = g.FirstName,
                LastName = g.LastName,
                BirthDate = g.BirthDate,
                BirthCity = g.BirthCity,
                Citizenship = g.Citizenship,
                Role = g.Role.ToString(),
                TaxCode = g.TaxCode,
                Address = g.Address,
                CityOfResidence = g.CityOfResidence,
                Province = g.Province,
                PostalCode = g.PostalCode,
                DocumentType = g.DocumentType,
                DocumentNumber = g.DocumentNumber,
                DocumentExpiration = g.DocumentExpiration,
                ReservationId = g.ReservationId,
                CreatedBy = GetUserName(g.CreatedBy),
                CreatedAt = g.CreatedAt,
                UpdatedBy = GetUserName(g.UpdatedBy),
                UpdatedAt = g.UpdatedAt,
                DeletedBy = GetUserName(g.DeletedBy),
                DeletedAt = g.DeletedAt
            }).ToList();

            // Map Payment DTOs
            var paymentDtos = reservation.Payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                ReservationId = p.ReservationId,
                Amount = p.Amount,
                Type = p.Type,
                Status = p.Status,
                PaymentMethodId = p.PaymentMethodId,
                PaymentMethodCode = p.PaymentMethod?.Code,
                PaymentMethodDescription = p.PaymentMethod?.Description,
                PaidAt = p.PaidAt,
                IsInvoiced = p.IsInvoiced,
                CreatedBy = GetUserName(p.CreatedBy),
                CreatedAt = p.CreatedAt,
                UpdatedBy = GetUserName(p.UpdatedBy),
                UpdatedAt = p.UpdatedAt,
                DeletedBy = GetUserName(p.DeletedBy),
                DeletedAt = p.DeletedAt
            }).ToList();

            // Map Charge DTOs
            var chargeDtos = reservation.Charges.Select(c => new ChargeDto
            {
                Id = c.Id,
                ReservationId = c.ReservationId,
                Description = c.Description,
                Type = c.Type.ToString(),
                UnitPrice = c.UnitPrice,
                VatRate = c.VatRate,
                Quantity = c.Quantity,
                Amount = c.Amount,
                IsInvoiced = c.IsInvoiced,
                CreatedBy = GetUserName(c.CreatedBy),
                CreatedAt = c.CreatedAt,
                UpdatedBy = GetUserName(c.UpdatedBy),
                UpdatedAt = c.UpdatedAt,
                DeletedBy = GetUserName(c.DeletedBy),
                DeletedAt = c.DeletedAt
            }).ToList();

            // Map Invoice DTOs
            var invoiceDtos = reservation.Invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                ReservationId = i.ReservationId,
                InvoiceNumber = i.InvoiceNumber,
                Status = i.Status,
                IssueDate = i.IssueDate,
                SubTotal = i.SubTotal,
                TaxAmount = i.TaxAmount,
                TotalAmount = i.TotalAmount,
                BalanceDue = i.TotalAmount - (i.InvoicePayments?.Sum(p => p.AmountApplied) ?? 0),
                RemainingAmount = i.RemainingAmount,

                Customer = i.Customer != null ? new InvoiceCustomerDto
                {
                    FirstName = i.Customer.FirstName,
                    LastName = i.Customer.LastName,
                    TaxCode = i.Customer.TaxCode,
                    Address = i.Customer.Address,
                    City = i.Customer.City,
                    Country = i.Customer.Country
                } : null,

                Items = i.Items?.Select(it => new InvoiceItemDto
                {
                    Id = it.Id,
                    Description = it.Description,
                    UnitPrice = it.UnitPrice,
                    Quantity = it.Quantity,
                    TotalPrice = it.TotalPrice,
                    VatRate = it.VatRate,
                    VatAmount = it.VatAmount
                }).ToList() ?? new List<InvoiceItemDto>(),

                Payments = i.InvoicePayments?.Select(p => new InvoicePaymentDto
                {
                    PaymentId = p.PaymentId,
                    AmountApplied = p.AmountApplied,
                    CreatedAt = p.CreatedAt
                }).ToList() ?? new List<InvoicePaymentDto>(),

                CreatedBy = GetUserName(i.CreatedBy),
                CreatedAt = i.CreatedAt,
                UpdatedBy = GetUserName(i.UpdatedBy),
                UpdatedAt = i.UpdatedAt,
                DeletedBy = GetUserName(i.DeletedBy),
                DeletedAt = i.DeletedAt
            }).ToList();

            // Final Reservation DTO
            return new ReservationDetailDto
            {
                Id = reservation.Id,
                CustomerName = $"{reservation.FirstName} {reservation.LastName}",
                Phone = reservation.Phone,
                Email = reservation.Email,
                RoomId = reservation.RoomId,
                RoomNumber = reservation.Room?.RoomNumber,
                CheckIn = reservation.CheckIn,
                CheckOut = reservation.CheckOut,
                Status = reservation.Status.ToString(),
                Note = reservation.Note,

                PaymentStatus = paymentStatus,
                RemainingAmount = remainingAmount,
                IsRoomInvoiced = reservation.IsRoomInvoiced,

                CreatedBy = GetUserName(reservation.CreatedBy),
                CreatedAt = reservation.CreatedAt,
                UpdatedBy = GetUserName(reservation.UpdatedBy),
                UpdatedAt = reservation.UpdatedAt,
                DeletedBy = GetUserName(reservation.DeletedBy),
                DeletedAt = reservation.DeletedAt,

                Guests = guestDtos,
                Payments = paymentDtos,
                Charges = chargeDtos,
                Invoices = invoiceDtos
            };
        }
        public async Task<List<ReservationListDto>> SearchReservationAsync(ReservationSearchDto dto)
        {
            var query = _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Room)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                var parts = dto.CustomerName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var temp = part;
                    query = query.Where(r => r.FirstName.Contains(temp) || r.LastName.Contains(temp));
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
                query = query.Where(r => r.Email.Contains(dto.Email));

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                query = query.Where(r => r.Phone.Contains(dto.Phone));

            if (dto.RoomId.HasValue)
                query = query.Where(r => r.RoomId == dto.RoomId.Value);

            if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<ReservationStatus>(dto.Status, out var status))
                query = query.Where(r => r.Status == status);

            if (dto.FromDate.HasValue)
                query = query.Where(r => r.CheckOut > dto.FromDate.Value.Date);

            if (dto.ToDate.HasValue)
                query = query.Where(r => r.CheckIn < dto.ToDate.Value.Date);

            return await query
                .Select(r => new ReservationListDto
                {
                    Id = r.Id,
                    CustomerName = r.FirstName + " " + r.LastName,
                    Phone = r.Phone,
                    Email = r.Email,
                    RoomId = r.RoomId,
                    RoomNumber = r.Room.RoomNumber,
                    CheckIn = r.CheckIn,
                    CheckOut = r.CheckOut,
                    Status = r.Status.ToString()
                })
                .ToListAsync();
        }

    }
}
