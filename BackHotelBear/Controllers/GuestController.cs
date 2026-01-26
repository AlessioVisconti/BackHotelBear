using BackHotelBear.Models.Dtos.GuestDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        //CREATE
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CreateGuest([FromBody] GuestDto dto)
        {
            var result = await _guestService.CreateGuestAsync(dto);

            if (!result.Success)
                return BadRequest(new { result.ErrorMessage });

            return Ok(result.Guest);
        }

        //UPDATE
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> UpdateGuest(Guid id, [FromBody] GuestDto dto)
        {
            var result = await _guestService.UpdateGuestAsync(id, dto);

            if (!result.Success)
                return NotFound(new { result.ErrorMessage });

            return Ok(result.Guest);
        }

        //DELETE
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> DeleteGuest(Guid id)
        {
            var result = await _guestService.DeleteGuestAsync(id);

            if (!result.Success)
                return NotFound(new { result.ErrorMessage });

            return NoContent();
        }

        //GET
        [HttpPost("search")]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<IActionResult> SearchGuests([FromBody] GuestResearchDto dto)
        {
            var guests = await _guestService.SearchGuestAsync(dto);
            return Ok(guests);
        }
    }
}
