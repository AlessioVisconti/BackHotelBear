using BackHotelBear.Models.Entity.ReservationAndEnum;

namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class ReservationBarDto
    {
        public Guid ReservationId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string GuestName { get; set; } = null!;
        public ReservationStatus Status { get; set; }
        public bool StartsBeforeRange { get; set; }
        public bool EndsAfterRange { get; set; }
    }
}