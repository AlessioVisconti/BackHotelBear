using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class RoomDetailDto
    {
        public Guid Id { get; set; }
        [Required, MaxLength(5)]
        public string RoomNumber { get; set; }
        [Required, MaxLength(30)]
        public string RoomName { get; set; } = null!;
        [Required, MaxLength(150)]
        public string? Description { get; set; }
        public int Beds { get; set; }
        public string BedsTypes { get; set; } = null!;
        public decimal PriceForNight { get; set; }
        public List<RoomPhotoDto> Photos { get; set; } = new();
    }
}
