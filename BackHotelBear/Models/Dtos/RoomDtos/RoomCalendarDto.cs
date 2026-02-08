namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class RoomCalendarDto
    {
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public decimal RoomPrice { get; set; }
        public List<ReservationBarDto> Reservations { get; set; } = new();
    }
}
