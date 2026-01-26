using BackHotelBear.Models.Dtos.AuthDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RegisterStaffAsync(CreateStaffUserDto dto);
        Task<List<UserDto>> GetAllCustomersAsync();
        Task<List<UserDto>> GetAllStaffAsync();
        Task<bool> SoftDeleteCustomerAsync(string userId);
        Task<bool> SoftDeleteStaffAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> ReactivateCustomerAsync(string userId);
        Task<bool> ReactivateStaffAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<string?> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
