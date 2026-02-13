using BackHotelBear.Models.Dtos.ChargeDtos;
using BackHotelBear.Models.Dtos.GuestDtos;
using BackHotelBear.Models.Dtos.InvoiceDtos;
using BackHotelBear.Models.Dtos.PaymentDtos;
using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.ReservationDtos
{
    public class ReservationDetailDto : ReservationListDto
    {
        [MaxLength(150)]
        public string? Note { get; set; }
        public List<PaymentDto> Payments { get; set; } = new();
        public List<ChargeDto> Charges { get; set; } = new();
        public List<GuestDto> Guests { get; set; } = new();
        public List<InvoiceDto> Invoices { get; set; } = new();
        public bool IsRoomInvoiced { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
