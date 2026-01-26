namespace BackHotelBear.Models.Dtos.GuestDtos
{
    public class GuestResult
    {
        public bool Success { get; set; }
        public GuestDto? Guest { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
