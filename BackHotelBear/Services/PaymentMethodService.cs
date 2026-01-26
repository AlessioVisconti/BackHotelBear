using BackHotelBear.Models.Data;
using BackHotelBear.Models.Dtos.PaymentMethodDtos;
using BackHotelBear.Models.Entity.PaymentAndEnum;
using BackHotelBear.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackHotelBear.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly HotelBearDbContext _context;
        public PaymentMethodService(HotelBearDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto)
        {
            var exists = await _context.PaymentMethods
                .AnyAsync(pm => pm.Code == dto.Code && pm.DeletedAt == null);

            if (exists)
                return null;

            var method = new PaymentMethod
            {
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                IsActive = true
            };

            _context.PaymentMethods.Add(method);
            await _context.SaveChangesAsync();

            return new PaymentMethodDto
            {
                Id = method.Id,
                Code = method.Code,
                Description = method.Description,
                IsActive = method.IsActive
            };
        }

        public async Task<PaymentMethodDto> UpdateAsync(Guid id, UpdatePaymentMethodDto dto)
        {
            var method = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == id && pm.DeletedAt == null);

            if (method == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                method.Description = dto.Description;

            if (dto.IsActive.HasValue)
                method.IsActive = dto.IsActive.Value;

            method.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PaymentMethodDto
            {
                Id = method.Id,
                Code = method.Code,
                Description = method.Description,
                IsActive = method.IsActive
            };
        }
        public async Task<bool> DeactivateAsync(Guid id)
        {
            var method = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == id && pm.DeletedAt == null);

            if (method == null)
                return false;

            method.IsActive = false;
            method.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PaymentMethodDto>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.PaymentMethods
                .Where(pm => pm.DeletedAt == null);

            if (!includeInactive)
                query = query.Where(pm => pm.IsActive);

            return await query
                .OrderBy(pm => pm.Description)
                .Select(pm => new PaymentMethodDto
                {
                    Id = pm.Id,
                    Code = pm.Code,
                    Description = pm.Description,
                    IsActive = pm.IsActive
                })
                .ToListAsync();
        }

    }
}
