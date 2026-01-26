using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Dtos.AuthDtos
{
    public class RegisterCustomerDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null;
    }
}
