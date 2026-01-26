using BackHotelBear.Models.Entity.GuestAndEnum;
using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.GuestDtos
{
    public class GuestDto
    {
        public Guid Id { get; set; }
        [Required]
        public Guid ReservationId { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [MaxLength(50)]

        public string LastName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string BirthCity { get; set; } = null!;
        public string Role { get; set; } = null!;//role inteso come tipo di guest se Single,HeadOfFamily etc.
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Il Codice Fiscale deve contenere esattamente 16 caratteri")]
        public string? TaxCode { get; set; }
        [MaxLength(50)]
        public string? Address { get; set; }
        [MaxLength(30)]
        public string? Citizenship { get; set; }
        [MaxLength(30)]
        public string? CityOfResidence { get; set; }
        [StringLength(2,MinimumLength =2)]
        public string? Province { get; set; }
        [StringLength(5, MinimumLength = 5)]
        public string? PostalCode { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public DateTime? DocumentExpiration { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
