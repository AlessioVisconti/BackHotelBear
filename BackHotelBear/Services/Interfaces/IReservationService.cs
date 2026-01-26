using BackHotelBear.Models.Dtos.ReservationDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationResult> CreateReservationAsync(CreateReservationDto dto, string? customerId = null);
        Task<ReservationResult> UpdateReservationAsync(Guid reservationId, UpdateReservationDto dtop, string? modifiedByUserId);
        Task<ReservationDetailDto> GetReservationByIdAsync(Guid reservationId);
        Task<ReservationResult> CancelReservationAsync(Guid reservationId, string? cancelledByUserId = null);//soft delete
        Task<List<ReservationListDto>> SearchReservationAsync(ReservationSearchDto dto);
    }
}
