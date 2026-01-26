using BackHotelBear.Models.Entity.PaymentAndEnum;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.PaymentDtos
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        public PaymentType Type { get; set; }
        public PaymentStatus Status { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string PaymentMethodCode { get; set; } = null!;
        public string PaymentMethodDescription { get; set; } = null!;
        public bool IsInvoiced { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? PaidAt { get; internal set; }
    }
}
