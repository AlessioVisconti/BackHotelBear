using BackHotelBear.Models.Dtos.ChargeDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeController : ControllerBase
    {
        private readonly IChargeService _chargeService;

        public ChargeController(IChargeService chargeService)
        {
            _chargeService = chargeService;
        }

        //CREATE-USATO
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChargeDto dto)
        {
            try
            {
                var result = await _chargeService.CreateChargeAsync(dto);
                return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //UPDATE-USATO
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ChargeDto dto)
        {
            try
            {
                var result = await _chargeService.UpdateChargeAsync(id, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //DELETE-USATO
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _chargeService.DeleteChargeAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
