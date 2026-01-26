namespace BackHotelBear.Models.Dtos.ReservationDtos
{
    public class ReservationResult
    {
        public bool Success { get; set; }
        public ReservationDetailDto? Reservation { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
