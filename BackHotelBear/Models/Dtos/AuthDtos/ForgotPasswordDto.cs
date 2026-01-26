using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.AuthDtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
