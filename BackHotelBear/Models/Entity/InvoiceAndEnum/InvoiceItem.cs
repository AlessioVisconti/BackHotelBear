using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Entity.InvoiceAndEnum
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }
        [Required]
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        [Required, MaxLength(100)]
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
