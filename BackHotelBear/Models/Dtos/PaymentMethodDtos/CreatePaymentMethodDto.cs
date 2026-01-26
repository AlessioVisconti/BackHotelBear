using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.PaymentMethodDtos
{
    public class CreatePaymentMethodDto
    {
        [Required, MaxLength(10)]
        public string Code { get; set; } = null!;
        [Required, MaxLength(50)]
        public string Description { get; set; } = null!;
    }
}
