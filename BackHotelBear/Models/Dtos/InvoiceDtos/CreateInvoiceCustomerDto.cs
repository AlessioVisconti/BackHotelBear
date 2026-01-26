using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.InvoiceDtos
{
    public class CreateInvoiceCustomerDto
    {
        [Required,MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required,MaxLength(50)]
        public string LastName { get; set; } = null!;
        [Required]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Il Codice Fiscale deve contenere esattamente 16 caratteri")]
        public string? TaxCode { get; set; }
        [MaxLength(5)]
        public string? VatNumber { get; set; }
        [MaxLength(50)]
        public string? Address { get; set; }
        [MaxLength(30)]
        public string? City { get; set; }
        [MaxLength(50)]
        public string? Country { get; set; }
    }

}
