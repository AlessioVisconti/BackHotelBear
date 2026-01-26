using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.InvoiceDtos
{
    public class CreateInvoiceItemDto
    {
        [Required, MaxLength(100)]
        public string Description { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(4,2)")]
        public decimal VatRate { get; set; }

    }
}
