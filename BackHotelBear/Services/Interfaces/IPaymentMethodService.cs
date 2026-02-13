using BackHotelBear.Models.Dtos.PaymentMethodDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<List<PaymentMethodDto>> GetAllAsync(bool includeInactive = false);
        Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto);
        Task<PaymentMethodDto> UpdateAsync(Guid id,UpdatePaymentMethodDto dto);
        Task<bool> DeactivateAsync(Guid id);
    }
}
