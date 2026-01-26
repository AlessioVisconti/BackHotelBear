using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.ReservationDtos
{
    public class UpdateReservationDto
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public Guid? RoomId { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        [MaxLength(150)]
        public string? Note { get; set; }
        public string? Status { get; set; }
    }
}
