using BackHotelBear.Models.Dtos.InvoiceDtos;
using BackHotelBear.Services;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        //CREATE-Used
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var invoiceId = await _invoiceService.CreateInvoiceAsync(dto);
                return CreatedAtAction(nameof(GetInvoiceById), new { invoiceId }, new { invoiceId });
            }
            catch (InvalidOperationException ex)
            {
                // ad esempio "non abbastanza pagamenti disponibili"
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        //CANCEL-used
        [HttpPost("{invoiceId}/cancel")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CancelInvoice(Guid invoiceId)
        {
            await _invoiceService.CancelInvoiceAsync(invoiceId);
            return NoContent();
        }

        //GET-used
        [HttpGet("{invoiceId}")]
        [Authorize(Roles = "Admin,Receptionist,RoomStaff")]
        public async Task<IActionResult> GetInvoiceById(Guid invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        //Get-used
        [HttpGet("{invoiceId}/pdf")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> GetInvoicePdf(Guid invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null) return NotFound();
            var pdfService = new InvoicePdfService();
            var filePath = await pdfService.GenerateInvoiceHtmlFileAsync(invoice);
            var absolutePath = Path.GetFullPath(filePath);

            if (!System.IO.File.Exists(absolutePath))
                return NotFound();
            return PhysicalFile(absolutePath, "text/html", Path.GetFileName(filePath));
        }
    }
}