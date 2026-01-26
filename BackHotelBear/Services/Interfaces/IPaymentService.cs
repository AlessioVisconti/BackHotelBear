using BackHotelBear.Models.Dtos.PaymentDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto?> CreatePaymentAsync(CreatePaymentDto dto);
        Task<PaymentDto?> UpdatePaymentAsync(Guid id, UpdatePaymentDto dto);
        Task<bool> DeletePaymentAsync(Guid paymentid);//soft delete
        Task<List<PaymentDto>> GetAllPaymentsAsync();
    }
}
