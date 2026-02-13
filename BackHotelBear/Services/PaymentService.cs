using BackHotelBear.Models.Data;
using BackHotelBear.Models.Dtos.PaymentDtos;
using BackHotelBear.Models.Entity.PaymentAndEnum;
using BackHotelBear.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackHotelBear.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HotelBearDbContext _context;
        public PaymentService(HotelBearDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentDto?> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId && r.DeletedAt == null);
            if (reservation == null)
                throw new ArgumentException("Reservation not found.");

            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == dto.PaymentMethodId && pm.DeletedAt == null && pm.IsActive);
            if (paymentMethod == null)
                throw new ArgumentException("Payment method not found or inactive.");

            if (dto.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var payment = new Payment
            {
                ReservationId = dto.ReservationId,
                Amount = dto.Amount,
                Type = dto.Type,
                PaymentMethodId = dto.PaymentMethodId,
                Status = dto.PaidAt.HasValue ? PaymentStatus.Completed : PaymentStatus.Pending,
                PaidAt = dto.PaidAt,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy ?? "System"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return await MapToDto(payment.Id);
        }

        public async Task<PaymentDto?> UpdatePaymentAsync(Guid paymentId, UpdatePaymentDto dto)
        {
            var payment = await _context.Payments
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.DeletedAt == null);

            if (payment == null)
                return null;

            if (dto.Amount.HasValue && dto.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (dto.PaymentMethodId.HasValue)
            {
                var method = await _context.PaymentMethods
                    .FirstOrDefaultAsync(pm => pm.Id == dto.PaymentMethodId.Value && pm.DeletedAt == null && pm.IsActive);

                if (method == null)
                    throw new ArgumentException("Payment method not found or inactive.");

                payment.PaymentMethodId = dto.PaymentMethodId.Value;
            }

            payment.Amount = dto.Amount ?? payment.Amount;
            payment.Type = dto.Type ?? payment.Type;
            payment.Status = dto.Status ?? payment.Status;
            payment.PaidAt = dto.PaidAt ?? payment.PaidAt;

            payment.UpdatedAt = DateTime.UtcNow;
            payment.UpdatedBy = dto.UpdatedBy ?? "System";

            await _context.SaveChangesAsync();

            return await MapToDto(payment.Id);
        }
        public async Task<bool> DeletePaymentAsync(Guid paymentId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && p.DeletedAt == null);
            if (payment == null) return false;

            payment.DeletedAt = DateTime.UtcNow;
            payment.DeletedBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }


        private async Task<PaymentDto?> MapToDto(Payment? payment)
        {
            if (payment == null) return null;

            return new PaymentDto
            {
                Id = payment.Id,
                ReservationId = payment.ReservationId,
                Amount = payment.Amount,
                Type = payment.Type,
                Status = payment.Status,
                PaymentMethodId = payment.PaymentMethodId,
                PaymentMethodCode = payment.PaymentMethod?.Code,
                PaymentMethodDescription = payment.PaymentMethod?.Description,
                PaidAt = payment.PaidAt,

                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt,
                DeletedAt = payment.DeletedAt,

                CreatedBy = await ResolveUserAsync(payment.CreatedBy),
                UpdatedBy = await ResolveUserAsync(payment.UpdatedBy),
                DeletedBy = await ResolveUserAsync(payment.DeletedBy)
            };
        }
        private async Task<PaymentDto?> MapToDto(Guid paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.DeletedAt == null);

            return await MapToDto(payment);
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
