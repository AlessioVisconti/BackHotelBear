using BackHotelBear.Models.Entity.ReservationAndEnum;
using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Entity.GuestAndEnum
{
    public class Guest : BaseEntity
    {
        public Guid Id { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;
        [Required]
        public DateTime BirthDate { get; set; }
        [Required, MaxLength(50)]
        public string BirthCity { get; set; } = null!;
        [Required, MaxLength(50)]
        public string Citizenship { get; set; } = null!;
        public GuestRole Role { get; set; }
        [StringLength(16, MinimumLength = 16, ErrorMessage = "The tax code must contain exactly 16 characters")]
        public string? TaxCode { get; set; }
        public string? Address { get; set; }
        public string? CityOfResidence { get; set; }
        [StringLength(2, MinimumLength = 2)]
        public string? Province { get; set; }
        [StringLength(5, MinimumLength = 5)]
        public string? PostalCode { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public DateTime? DocumentExpiration { get; set; }
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
    }
}
