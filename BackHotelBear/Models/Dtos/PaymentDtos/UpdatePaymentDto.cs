using BackHotelBear.Models.Entity.PaymentAndEnum;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.PaymentDtos
{
    public class UpdatePaymentDto
    {
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }
        public PaymentType? Type { get; set; }
        public PaymentStatus? Status { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
