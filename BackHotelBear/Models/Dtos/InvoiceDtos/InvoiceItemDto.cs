using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.InvoiceDtos
{
    public class InvoiceItemDto
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public decimal VatRate { get; set; }
        public decimal VatAmount { get; set; }
    }
}
