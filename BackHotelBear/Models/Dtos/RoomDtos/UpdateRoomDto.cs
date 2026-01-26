using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.RoomDtos
{
    public class UpdateRoomDto
    {
        [MaxLength(5)]
        public string? RoomNumber { get; set; }
        [MaxLength(30)]
        public string? RoomName { get; set; } = null!;
        [MaxLength(150)]
        public string? Description { get; set; }
        public int? Beds { get; set; }
        public string? BedsTypes { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")]
        public decimal? PriceForNight { get; set; }
    }
}
