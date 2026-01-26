using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.InvoiceDtos
{
    public class InvoicePaymentDto
    {
        public Guid PaymentId { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountApplied { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
