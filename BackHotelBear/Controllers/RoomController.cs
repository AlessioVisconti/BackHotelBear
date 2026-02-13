using BackHotelBear.Models.Dtos.RoomDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        //CREATE-Used
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            var room = await _roomService.CreateRoomAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        //UPDATE-Used
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomDto dto)
        {
            try
            {
                var room = await _roomService.UpdateRoomAsync(id, dto);
                if (room == null) return NotFound();
                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //DELETE-Used
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _roomService.DeleteRoomAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        //GET-Used
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _roomService.GetRoomDetailAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        //GET-Used
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }
        //Used
        [HttpGet("calendar")]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<ActionResult<List<RoomCalendarDto>>> GetRoomCalendar(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
        {
            var result = await _roomService.GetRoomCalendarAsync(startDate, endDate);
            return Ok(result);
        }
        //Used
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkIn,[FromQuery] DateTime checkOut)
        {
            if (checkIn >= checkOut)
                return BadRequest("Check-out must be after check-in.");

            var rooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut);

            return Ok(rooms);
        }

        //FOTO
        //CREATE(ADD)-Used
        [HttpPost("{id}/photos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPhoto(Guid id, [FromForm] AddRoomPhotoDto dto)
        {
            await _roomService.AddRoomPhotoAsync(id, dto);
            return Ok();
        }

        //DELETE-Used
        [HttpDelete("photos/{photoId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhoto(Guid photoId)
        {
            var deleted = await _roomService.DeleteRoomPhotoAsync(photoId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // SET COVER PHOTO-Used
        [HttpPatch("photos/{photoId}/cover")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetCover(Guid photoId)
        {
            var updated = await _roomService.SetCoverPhotoAsync(photoId);

            if (!updated) return NotFound();

            return NoContent();
        }


    }
}
