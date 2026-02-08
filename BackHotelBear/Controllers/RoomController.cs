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

        //CREATE
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            var room = await _roomService.CreateRoomAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        //UPDATE
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
                // ritorna messaggio reale per debug
                return StatusCode(500, ex.Message);
            }
        }

        //DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _roomService.DeleteRoomAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        //GET
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _roomService.GetRoomDetailAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }
        //usato
        [HttpGet("calendar")]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<ActionResult<List<RoomCalendarDto>>> GetRoomCalendar(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
        {
            var result = await _roomService.GetRoomCalendarAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("calendar/check")]
        public async Task<ActionResult<RoomDayClickResultDto>> CheckRoomDay(Guid roomId, DateTime day)
        {
            var result = await _roomService
                .CheckRoomAvailabilityAsync(roomId, day);

            return Ok(result);
        }

        //FOTO
        //CREATE(ADD)
        [HttpPost("{id}/photos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPhoto(Guid id, [FromForm] AddRoomPhotoDto dto)
        {
            await _roomService.AddRoomPhotoAsync(id, dto);
            return Ok();
        }

        //DELETE
        [HttpDelete("photos/{photoId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhoto(Guid photoId)
        {
            var deleted = await _roomService.DeleteRoomPhotoAsync(photoId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // SET COVER PHOTO
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
