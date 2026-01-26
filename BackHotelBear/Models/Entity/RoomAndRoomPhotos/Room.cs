using BackHotelBear.Models.Entity.ReservationAndEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Entity.RoomAndRoomPhotos
{
    public class Room
    {
        public Guid Id { get; set; }
        [Required, MaxLength(5)]
        public string RoomNumber { get; set; } = null;
        [Required, MaxLength(30)]
        public string RoomName { get; set; } = null!;
        [MaxLength(150)]
        public string? Description { get; set; }
        public int Beds { get; set; }
        [MaxLength(20)]
        public string BedsTypes { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")]

        public decimal PriceForNight { get; set; }
        public ICollection<RoomPhoto> Photos { get; set; } = new List<RoomPhoto>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
