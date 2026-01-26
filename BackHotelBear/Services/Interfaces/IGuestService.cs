using BackHotelBear.Models.Dtos.GuestDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IGuestService
    {
        Task<GuestResult> CreateGuestAsync(GuestDto dto);
        Task<GuestResult> UpdateGuestAsync(Guid guestId, GuestDto dto);
        Task<GuestResult> DeleteGuestAsync(Guid guestId);
        Task<List<GuestDto>> SearchGuestAsync(GuestResearchDto dto);
    }
}
