using BackHotelBear.Models.Data;
using BackHotelBear.Models.Dtos.ChargeDtos;
using BackHotelBear.Models.Entity.ChargeAndEnum;
using BackHotelBear.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackHotelBear.Services
{
    public class ChargeService : IChargeService
    {
        private readonly HotelBearDbContext _context;
        public ChargeService(HotelBearDbContext context)
        {
            _context = context;
        }
        public async Task<ChargeDto> CreateChargeAsync(ChargeDto dto)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId && r.DeletedAt == null);

            if (reservation == null)
                throw new ArgumentException("Reservation not found");

            var charge = new Charge
            {
                ReservationId = dto.ReservationId,
                Description = dto.Description,
                Type = Enum.Parse<ChargeType>(dto.Type),
                UnitPrice = dto.UnitPrice,
                Quantity = dto.Quantity,
                Amount = dto.UnitPrice * dto.Quantity,
                VatRate = dto.VatRate,
                IsInvoiced = dto.IsInvoiced
            };
            _context.Charges.Add(charge);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(charge);
        }
        public async Task<ChargeDto> UpdateChargeAsync(Guid chargeId, ChargeDto dto)
        {
            var charge = await _context.Charges
                .FirstOrDefaultAsync(c => c.Id == chargeId && c.DeletedAt == null);

            if (charge == null)
                throw new ArgumentException("Charge not found");

            charge.Description = dto.Description;
            charge.Type = Enum.Parse<ChargeType>(dto.Type);
            charge.UnitPrice = dto.UnitPrice;
            charge.Quantity = dto.Quantity;
            charge.Amount = dto.UnitPrice * dto.Quantity;
            charge.VatRate = dto.VatRate;
            charge.IsInvoiced = dto.IsInvoiced;

            await _context.SaveChangesAsync();
            return await MapToDtoAsync(charge);
        }

        public async Task DeleteChargeAsync(Guid chargeId)
        {
            var charge = await _context.Charges
                .FirstOrDefaultAsync(c => c.Id == chargeId);

            if (charge == null)
                throw new ArgumentException("Charge not found");

            if (charge.IsInvoiced)
                throw new InvalidOperationException("Cannot delete a charge that has been invoiced.");

            _context.Charges.Remove(charge);
            await _context.SaveChangesAsync();
        }


        private async Task<ChargeDto> MapToDtoAsync(Charge c)
        {
            string createdBy = c.CreatedBy ?? "System";
            if (!string.IsNullOrEmpty(c.CreatedBy))
            {
                var createdUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == c.CreatedBy);

                if (createdUser != null)
                    createdBy = createdUser.FullName;
            }
            string? updatedBy = null;
            if (!string.IsNullOrEmpty(c.UpdatedBy))
            {
                var updatedUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == c.UpdatedBy);

                if (updatedUser != null)
                    updatedBy = updatedUser.FullName;
            }

            return new ChargeDto
            {
                Id = c.Id,
                ReservationId = c.ReservationId,
                Description = c.Description,
                Type = c.Type.ToString(),
                UnitPrice = c.UnitPrice,
                Quantity = c.Quantity,
                Amount = c.Amount,
                VatRate = c.VatRate,
                IsInvoiced = c.IsInvoiced,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy
            };
        }
    }
}
