using BackHotelBear.Models.Entity.ReservationAndEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Entity.ChargeAndEnum
{
    public class Charge : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        [Required]
        public string Description { get; set; } = null!;
        public ChargeType Type { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        [Required]
        [Column(TypeName = "decimal(4,2)")]
        public decimal VatRate { get; set; }
        public bool IsInvoiced { get; set; } = false;
    }
}
