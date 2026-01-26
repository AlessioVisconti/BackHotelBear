using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.ChargeDtos
{
    public class ChargeDto
    {
        public Guid Id { get; set; }
        [Required]
        public Guid ReservationId { get; set; }
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string Type { get; set; } = null!;//sarebbe charge type, probabilmente funziona anche se al posto di string richiami proprio ChargeType
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        [Required]
        [Column(TypeName = "decimal(4,2)")]

        public decimal VatRate { get; set; }
        public bool IsInvoiced { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
