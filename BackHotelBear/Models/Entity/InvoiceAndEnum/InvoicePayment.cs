using BackHotelBear.Models.Entity.PaymentAndEnum;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Entity.InvoiceAndEnum
{
    public class InvoicePayment
    {
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountApplied { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
