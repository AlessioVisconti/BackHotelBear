using BackHotelBear.Models.Dtos.InvoiceDtos;

namespace BackHotelBear.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<Guid> CreateAndIssueInvoiceFromReservationAsync(Guid reservationId, InvoiceCustomerDto customer);
        Task<bool> CancelInvoiceAsync(Guid invoiceId);
        Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId);

    }
}
