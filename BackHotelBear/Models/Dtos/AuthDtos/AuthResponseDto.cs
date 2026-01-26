namespace BackHotelBear.Models.Dtos.AuthDtos
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Expiration { get; set; }
    }
}
