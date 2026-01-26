using BackHotelBear.Models.Entity.ChargeAndEnum;
using BackHotelBear.Models.Entity.GuestAndEnum;
using BackHotelBear.Models.Entity.InvoiceAndEnum;
using BackHotelBear.Models.Entity.PaymentAndEnum;
using BackHotelBear.Models.Entity.RoomAndRoomPhotos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackHotelBear.Models.Entity.ReservationAndEnum
{
    public class Reservation : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? CustomerId { get; set; }
        [JsonIgnore]
        public User? Customer { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;
        [Required, Phone]
        public string Phone { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public Guid RoomId { get; set; }
        [JsonIgnore]
        public Room Room { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public bool IsRoomInvoiced { get; set; }
        [MaxLength(150)]
        public string? Note { get; set; }
        public ReservationPaymentStatus PaymentStatus { get; set; } = ReservationPaymentStatus.NotPaid;
        [JsonIgnore]
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        [JsonIgnore]
        public ICollection<Charge> Charges { get; set; } = new List<Charge>();
        [JsonIgnore]
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
        [JsonIgnore]
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    }
}
