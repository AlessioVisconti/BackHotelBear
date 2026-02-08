using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.ReservationDtos
{
    public class CreateReservationDto
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;
        [Required, Phone]
        public string Phone { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public Guid RoomId { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Price { get; set; }
        [MaxLength(150)]
        public string? Note { get; set; }
    }
}
