using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.PaymentMethodDtos
{
    public class UpdatePaymentMethodDto
    {
        [Required, MaxLength(50)]
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
