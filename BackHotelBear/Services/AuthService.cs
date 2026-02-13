using BackHotelBear.Models.Dtos.AuthDtos;
using BackHotelBear.Models.Entity;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackHotelBear.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        
        private string GenerateTokenJWT(User user)
        {
            var setting = _configuration.GetSection("JwtSettings");

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting["SecretKey"]!));

            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {

                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
           };

            var token = new JwtSecurityToken(
                issuer: setting["Issuer"],
                audience: setting["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(setting["ExpirationMinutes"]!)),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto)
        {

            var existingUser = await _userManager.FindByNameAsync(dto.Email);
            if (existingUser != null)
                return null;


            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null)
                return null;

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = "Customer",
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return null;


            await _userManager.AddToRoleAsync(user, "Customer");


            var token = GenerateTokenJWT(user);


            return new AuthResponseDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtSettings:ExpirationMinutes"]!))
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return null;


            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return null;


            if (!user.IsActive)
                return null;


            var token = GenerateTokenJWT(user);


            return new AuthResponseDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtSettings:ExpirationMinutes"]!))
            };
        }

        public async Task<AuthResponseDto> RegisterStaffAsync(CreateStaffUserDto dto)
        {

            var existingUser = await _userManager.FindByNameAsync(dto.Email);
            if (existingUser != null)
                return null;


            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null)
                return null;


            var allowedRoles = new[] { "Receptionist", "RoomStaff" };
            if (!allowedRoles.Contains(dto.Role))
                return null;


            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = dto.Role,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return null;


            await _userManager.AddToRoleAsync(user, dto.Role);


            var token = GenerateTokenJWT(user);


            return new AuthResponseDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtSettings:ExpirationMinutes"]!))
            };
        }

        public async Task<List<UserDto>> GetAllCustomersAsync()
        {
            var userDtos = await _userManager.Users
                .Where(u => u.Role == "Customer")
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return userDtos;
        }

        public async Task<List<UserDto>> GetAllStaffAsync()
        {
            var staffRoles = new[] { "Receptionist", "RoomStaff" };

            var userDtos = await _userManager.Users
                .Where(u => staffRoles.Contains(u.Role))
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return userDtos;
        }
        public async Task<bool> SoftDeleteCustomerAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.Role != "Customer")
                return false;

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> SoftDeleteStaffAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || (user.Role != "Receptionist" && user.Role != "RoomStaff"))
                return false;

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ReactivateCustomerAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.Role != "Customer")
                return false;

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> ReactivateStaffAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || (user.Role != "Receptionist" && user.Role != "RoomStaff"))
                return false;

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }
        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !user.IsActive)
                return null;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return resetToken;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
    }
}
