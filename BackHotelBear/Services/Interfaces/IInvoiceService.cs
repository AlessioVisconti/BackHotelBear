using BackHotelBear.Models.Dtos.InvoiceDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<bool> CancelInvoiceAsync(Guid invoiceId);
        Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId);
        Task<Guid> CreateInvoiceAsync(CreateInvoiceDto dto);

    }
}
