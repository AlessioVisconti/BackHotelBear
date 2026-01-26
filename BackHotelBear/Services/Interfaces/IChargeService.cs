using BackHotelBear.Models.Dtos.ChargeDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IChargeService
    {
        Task<ChargeDto> CreateChargeAsync(ChargeDto dto);
        Task<ChargeDto> UpdateChargeAsync(Guid chargeId, ChargeDto dto);
        Task DeleteChargeAsync(Guid chargeId);
    }
}
