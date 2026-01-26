using BackHotelBear.Models.Dtos.RoomDtos;
using BackHotelBear.Models.Entity.RoomAndRoomPhotos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IRoomService
    {
        Task<Room> CreateRoomAsync(CreateRoomDto dto);
        Task<Room?> UpdateRoomAsync(Guid roomId,UpdateRoomDto dto);
        Task<bool> DeleteRoomAsync(Guid roomdId);
        Task<List<RoomListDto>> GetAllRoomsAsync();
        Task AddRoomPhotoAsync(Guid roomId, AddRoomPhotoDto dto);
        Task<bool> DeleteRoomPhotoAsync(Guid photoId);
        Task<RoomDetailDto> GetRoomDetailAsync(Guid roomId);
        Task<List<RoomAvailabilityDto>> GetRoomOccupiedDatesAsync(Guid roomId);
        Task<List<RoomCalendarDto>> GetRoomCalendarAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<RoomDayClickResultDto> CheckRoomAvailabilityAsync(Guid roomId, DateTime day);
    }
}
