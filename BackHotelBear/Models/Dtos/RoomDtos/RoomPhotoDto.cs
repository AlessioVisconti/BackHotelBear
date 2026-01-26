namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class RoomPhotoDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public bool IsCover { get; set; }
    }
}
