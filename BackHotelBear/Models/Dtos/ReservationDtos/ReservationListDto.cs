using BackHotelBear.Models.Entity.ReservationAndEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackHotelBear.Models.Dtos.ReservationDtos
{
    public class ReservationListDto
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string CustomerName { get; set; } = null!;
        [Phone]
        public string Phone { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; } = null!;
        public ReservationPaymentStatus PaymentStatus { get; set; }
        [Column(TypeName = "decimal(10,2)")]

        public decimal RemainingAmount { get; set; }
    }
}
