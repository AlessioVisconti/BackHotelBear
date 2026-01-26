using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.InvoiceDtos
{
    public class CreateInvoiceDto
    {
        [Required]
        public Guid ReservationId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Required]
        public CreateInvoiceCustomerDto Customer { get; set; } = null!;

        public List<CreateInvoiceItemDto> Items { get; set; } = new List<CreateInvoiceItemDto>();
    }
}
