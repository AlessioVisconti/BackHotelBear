using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.PaymentMethodDtos
{
    public class PaymentMethodDto
    {
        public Guid Id { get; set; }
        [Required, MaxLength(10)]
        public string Code { get; set; } = null!;
        [Required, MaxLength(50)]
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
