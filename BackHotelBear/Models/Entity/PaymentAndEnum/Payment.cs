using BackHotelBear.Models.Entity.ReservationAndEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Entity.PaymentAndEnum
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
    public enum PaymentType
    {
        Deposit,
        Balance,
        Extra
    }
    public class Payment : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public Reservation Reservatio { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        public PaymentType Type { get; set; }
        public Guid PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = null!;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime? PaidAt { get; set; }
    }
}
