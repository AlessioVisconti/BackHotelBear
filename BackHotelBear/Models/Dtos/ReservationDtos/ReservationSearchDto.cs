using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.ReservationDtos
{
    public class ReservationSearchDto
    {
        [MaxLength(100)]
        public string? CustomerName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public Guid? RoomId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
    }
}
