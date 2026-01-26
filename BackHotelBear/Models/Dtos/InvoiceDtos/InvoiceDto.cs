using BackHotelBear.Models.Entity.InvoiceAndEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.InvoiceDtos
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        [Required]
        public Guid ReservationId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public InvoiceStatus Status { get; set; }
        public DateTime IssueDate { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal BalanceDue { get; set; }
        public decimal RemainingAmount { get; set; }
        public InvoiceCustomerDto Customer { get; set; } = null!;
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
        public List<InvoicePaymentDto> Payments { get; set; } = new List<InvoicePaymentDto>();
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
