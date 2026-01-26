namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class AddRoomPhotoDto
    {
        public IFormFile File { get; set; } = null!;
        public bool IsCover { get; set; }
    }
}
