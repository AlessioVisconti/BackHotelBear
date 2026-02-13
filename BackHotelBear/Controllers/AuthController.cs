using BackHotelBear.Models.Dtos.AuthDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //REGISTER CUSTOMER-used
        [HttpPost("register/customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterCustomerAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Email already exists" });

            return Ok(result);
        }

        //LOGIN-used
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid credentials or account deactivated" });

            return Ok(result);
        }

        //REGISTER STAFF-used
        [HttpPost("register/staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterStaff([FromBody] CreateStaffUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterStaffAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Error creating staff account" });

            return Ok(result);
        }

        //GET-For future
        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var users = await _authService.GetAllCustomersAsync();
            return Ok(users);
        }
        //Get Staff-used
        [HttpGet("staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStaff()
        {
            var users = await _authService.GetAllStaffAsync();
            return Ok(users);
        }

        //SOFT DELETE CUSTOMER-For future
        [HttpPut("customers/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateCustomer(string id)
        {
            var success = await _authService.SoftDeleteCustomerAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }
        //REACTIVATE CUSTOMER-For future
        [HttpPut("customers/{id}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReactivateCustomer(string id)
        {
            var success = await _authService.ReactivateCustomerAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //SOFT DELETE STAFF-Used
        [HttpPut("staff/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateStaff(string id)
        {
            var success = await _authService.SoftDeleteStaffAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }
        //REACTIVATE STAFF-USED
        [HttpPut("staff/{id}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReactivateStaff(string id)
        {
            var success = await _authService.ReactivateStaffAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //DELETE USER-For future
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _authService.DeleteUserAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //CHANGE PASSWORD-For future
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _authService.ChangePasswordAsync(
                userId,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!success)
                return BadRequest(new { message = "Password non valida" });

            return NoContent();
        }
        //Forgot Password-For future
        [HttpPost("forgot-password")]
        [Authorize]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authService.ForgotPasswordAsync(dto.Email);

            if (token == null)
                return NotFound(new { message = "User not found or inactive" });

            return Ok(new
            {
                message = "Password reset token generated",
                token = token
            });
        }
        //Reset Password-For future
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.ResetPasswordAsync(
                dto.Email,
                dto.Token,
                dto.NewPassword
            );

            if (!success)
                return BadRequest(new { message = "Invalid or expired token" });

            return NoContent();
        }
    }
}
