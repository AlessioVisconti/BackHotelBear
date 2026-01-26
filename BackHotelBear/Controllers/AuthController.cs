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

        //REGISTER CUSTOMER
        [HttpPost("register/customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterCustomerAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Email già esistente" });

            return Ok(result);
        }

        //LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Credenziali non valide o account disattivato" });

            return Ok(result);
        }

        //REGISTER STAFF
        [HttpPost("register/staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterStaff([FromBody] CreateStaffUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterStaffAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Errore nella creazione dell'account staff" });

            return Ok(result);
        }

        //GET
        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var users = await _authService.GetAllCustomersAsync();
            return Ok(users);
        }

        [HttpGet("staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStaff()
        {
            var users = await _authService.GetAllStaffAsync();
            return Ok(users);
        }

        //SOFT DELETE CUSTOMER
        [HttpPut("customers/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateCustomer(string id)
        {
            var success = await _authService.SoftDeleteCustomerAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPut("customers/{id}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReactivateCustomer(string id)
        {
            var success = await _authService.ReactivateCustomerAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //SOFT DELETE STAFF
        [HttpPut("staff/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateStaff(string id)
        {
            var success = await _authService.SoftDeleteStaffAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPut("staff/{id}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReactivateStaff(string id)
        {
            var success = await _authService.ReactivateStaffAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //DELETE USER
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _authService.DeleteUserAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //CHANGE PASSWORD
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

        [HttpPost("forgot-password")]
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
                return BadRequest(new { message = "Token non valido o scaduto" });

            return NoContent();
        }
    }
}
