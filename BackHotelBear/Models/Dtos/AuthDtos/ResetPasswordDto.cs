using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.AuthDtos
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Token { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
    }
}
