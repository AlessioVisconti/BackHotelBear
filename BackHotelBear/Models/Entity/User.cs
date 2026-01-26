using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BackHotelBear.Models.Entity
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public string FullName => $"{FirstName} {LastName}";
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [MaxLength(50)]
        public string LastName { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
