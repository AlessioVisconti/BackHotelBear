using BackHotelBear.Models.Entity.ReservationAndEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Entity.InvoiceAndEnum
{
        public enum InvoiceStatus
        {
            Draft,
            Issued,
            Cancelled
        }
    public class Invoice : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
        [Required, MaxLength(20)]
        public string InvoiceNumber { get; set; } = null!;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public DateTime IssueDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal RemainingAmount { get; set; }
        public Guid CustomerId { get; set; }
        public InvoiceCustomer Customer { get; set; } = null!;
        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public ICollection<InvoicePayment> InvoicePayments { get; set; } = new List<InvoicePayment>();
    }
}
