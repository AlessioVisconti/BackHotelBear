namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class RoomDayClickResultDto
    {
        public bool IsOccupied { get; set; }
        public Guid? ReservationId { get; set; }
    }
}
