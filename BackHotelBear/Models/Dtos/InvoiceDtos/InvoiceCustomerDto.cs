using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.InvoiceDtos
{
    public class InvoiceCustomerDto
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;
        [MaxLength(16)]
        public string TaxCode { get; set; }
        [MaxLength(100)]
        public string? Address { get; set; }
        [MaxLength(50)]
        public string? City { get; set; }
        [MaxLength(50)]
        public string? Country { get; set; }
    }
}
