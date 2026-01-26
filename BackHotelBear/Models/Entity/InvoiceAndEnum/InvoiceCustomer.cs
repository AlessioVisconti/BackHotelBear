using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Entity.InvoiceAndEnum
{
    public class InvoiceCustomer
    {
        [Key]
        [ForeignKey("Invoice")]
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;
        [StringLength(16, MinimumLength = 16, ErrorMessage = "The tax code must contain exactly 16 characters.")]
        public string? TaxCode { get; set; }
        [MaxLength(5)]
        public string? VatNumber { get; set; }
        [MaxLength(30)]
        public string? City { get; set; }
        [MaxLength(50)]
        public string? Address { get; set; }
        [MaxLength(50)]
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
