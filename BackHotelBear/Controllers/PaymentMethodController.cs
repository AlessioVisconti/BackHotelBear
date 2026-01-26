using BackHotelBear.Models.Dtos.PaymentMethodDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _service;

        public PaymentMethodController(IPaymentMethodService service)
        {
            _service = service;
        }

        //GET
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var methods = await _service.GetAllAsync(includeInactive);
            return Ok(methods);
        }

        //CREATE
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentMethodDto dto)
        {
            var method = await _service.CreateAsync(dto);
            if (method == null)
                return BadRequest("Payment method with this code already exists.");
            return Ok(method);
        }

        //UPDATE
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentMethodDto dto)
        {
            var method = await _service.UpdateAsync(id, dto);
            if (method == null)
                return NotFound();
            return Ok(method);
        }

        //DEACTIVAZE
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var success = await _service.DeactivateAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
