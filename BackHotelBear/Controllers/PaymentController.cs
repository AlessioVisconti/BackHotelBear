using BackHotelBear.Models.Dtos.PaymentDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        //CREATE-Used
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
        {
            try
            {
                var payment = await _service.CreatePaymentAsync(dto);
                return Ok(payment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }

        }

        //UPDATE-Used
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentDto dto)
        {
            try
            {
                var payment = await _service.UpdatePaymentAsync(id, dto);
                if (payment == null) return NotFound();
                return Ok(payment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        //DELETE-Used
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeletePaymentAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
