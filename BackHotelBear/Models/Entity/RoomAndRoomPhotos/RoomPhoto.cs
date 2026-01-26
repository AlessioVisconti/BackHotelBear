using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Entity.RoomAndRoomPhotos
{
    public class RoomPhoto
    {
        public Guid Id { get; set; }
        [Required]
        public string Url { get; set; } = null!;
        public bool IsCover { get; set; } = false;
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
    }
}
