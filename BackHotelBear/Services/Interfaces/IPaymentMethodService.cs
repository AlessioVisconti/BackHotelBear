using BackHotelBear.Models.Dtos.PaymentMethodDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<List<PaymentMethodDto>> GetAllAsync(bool includeInactive = false);
        Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto);
        Task<PaymentMethodDto> UpdateAsync(Guid id,UpdatePaymentMethodDto dto);
        Task<bool> DeactivateAsync(Guid id);
        //ci sto ancora ragionando su, probabilmente non faccio un Deactivate ma semplicemente tramite l'update decido se tenere il metodo o disattivarlo.
    }
}
