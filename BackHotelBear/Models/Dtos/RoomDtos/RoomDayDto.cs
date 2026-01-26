namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class RoomDayDto
    {
        public DateTime Date { get; set; }
        public bool IsOccupied { get; set; }
        public Guid? ReservationId { get; set; }
    }
}