namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class RoomCalendarDto
    {
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public List<RoomDayDto> Days { get; set; } = new();
    }
}
