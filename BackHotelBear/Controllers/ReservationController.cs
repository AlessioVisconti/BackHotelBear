using BackHotelBear.Models.Dtos.ReservationDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        //CREATE
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            string? customerId = null;

            // Se l'utente è loggato e ha ruolo Admin/Receptionist, può associare la prenotazione a un cliente
            if (User.Identity?.IsAuthenticated == true &&
            (User.IsInRole("Admin") || User.IsInRole("Receptionist")))
            {
                // Prende il claim con l'ID dell'utente loggato
                customerId = User.FindFirst("sub")?.Value
                             ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            }

            var result = await _reservationService.CreateReservationAsync(dto, customerId);

            if (!result.Success)
                return BadRequest(new { error = result.ErrorMessage });

            return CreatedAtAction(nameof(GetReservationById), new { id = result.Reservation!.Id }, result.Reservation);
        }

        //UPDATE
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> UpdateReservation(Guid id, [FromBody] UpdateReservationDto dto)
        {
            var userId = User.Identity?.Name; // traccia chi modifica

            var result = await _reservationService.UpdateReservationAsync(id, dto, userId);

            if (!result.Success)
                return NotFound(new { error = result.ErrorMessage });

            return Ok(result.Reservation);
        }

        //GET
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        //SOFT DELETE
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            var userId = User.Identity?.Name;

            var result = await _reservationService.CancelReservationAsync(id, userId);

            if (!result.Success)
                return NotFound(new { error = result.ErrorMessage });

            return NoContent();
        }

        //SEARCH
        [HttpPost("search")]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<IActionResult> SearchReservations([FromBody] ReservationSearchDto dto)
        {
            var reservations = await _reservationService.SearchReservationAsync(dto);
            return Ok(reservations);
        }
    }
}
