using BackHotelBear.Models.Entity.PaymentAndEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.PaymentDtos
{
    public class CreatePaymentDto
    {
        [Required]
        public Guid ReservationId { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
        [Required]
        public PaymentType Type { get; set; }
        [Required]
        public Guid PaymentMethodId { get; set; }
        public string? CreatedBy { get; internal set; }
    }
}
