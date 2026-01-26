using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.AuthDtos
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
