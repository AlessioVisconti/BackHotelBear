using BackHotelBear.Models.Dtos.InvoiceDtos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackHotelBear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;

        }
        
        //CREATE
        [HttpPost("from-reservation/{reservationId}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CreateInvoiceFromReservation(Guid reservationId, [FromBody] InvoiceCustomerDto customer)
        {
            var invoiceId = await _invoiceService.CreateAndIssueInvoiceFromReservationAsync(reservationId, customer);
            return CreatedAtAction(nameof(GetInvoiceById), new { invoiceId }, new { invoiceId });
        }

        //CANCEL
        [HttpPost("{invoiceId}/cancel")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CancelInvoice(Guid invoiceId)
        {
            await _invoiceService.CancelInvoiceAsync(invoiceId);
            return NoContent();
        }

        //GET
        [HttpGet("{invoiceId}")]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<IActionResult> GetInvoiceById(Guid invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }
    }
}
