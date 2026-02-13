using BackHotelBear.Models.Dtos.InvoiceDtos;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BackHotelBear.Services
{
    public class InvoicePdfService
    {
        private readonly string _outputFolder;

        public InvoicePdfService(string outputFolder = "Invoices")
        {
            _outputFolder = outputFolder;

            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }
        }

        public async Task<string> GenerateInvoiceHtmlFileAsync(InvoiceDto invoice)
        {
            var fileName = $"Invoice_{invoice.InvoiceNumber}.html";
            var filePath = Path.Combine(_outputFolder, fileName);

            var htmlContent = GenerateInvoiceHtml(invoice);

            await File.WriteAllTextAsync(filePath, htmlContent);

            return filePath;
        }

        private string GenerateInvoiceHtml(InvoiceDto invoice)
        {
            var html = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
            <meta charset='UTF-8'>
            <title>Invoice {invoice.InvoiceNumber}</title>
            <style>
                body {{ font-family: Arial, sans-serif; margin: 20px; }}
                h1 {{ text-align: center; }}
                h3 {{ margin-bottom: 5px; }}
                table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                th {{ background-color: #f2f2f2; }}
                .totals {{ margin-top: 20px; width: 300px; float: right; }}
                .totals table {{ border: none; }}
                .totals th, .totals td {{ border: none; text-align: right; }}
            </style>
            </head>
            <body>
            <h1>Hotel Bear</h1>
            <h2>Invoice #{invoice.InvoiceNumber}</h2>

            <h3>Customer</h3>
            <p>
            {invoice.Customer.FirstName} {invoice.Customer.LastName}<br>
            {invoice.Customer.Address}<br>
            {invoice.Customer.City}, {invoice.Customer.Country}<br>
            TaxCode: {invoice.Customer.TaxCode}
            </p>

            <h3>Invoice Details</h3>
            <p>
            Data: {invoice.IssueDate:dd/MM/yyyy}<br>
            </p>

            <table>
            <thead>
            <tr>
            <th>Description</th>
            <th>Quantity</th>
            <th>Price</th>
            <th>VAT</th>
            <th>Total</th>
            </tr>
            </thead>
            <tbody>
            ";

            foreach (var item in invoice.Items){
            html += $@"<tr>
            <td>{item.Description}</td>
            <td>{item.Quantity}</td>
            <td>{item.UnitPrice:C}</td>
            <td>{item.VatRate}%</td>
            <td>{item.TotalPrice:C}</td>
            </tr>";}
             html += $@"</tbody>
            </table>
            <div class='totals'>
            <table>
            <tr><th>Subtotal:</th><td>{invoice.SubTotal:C}</td></tr>
            <tr><th>VAT:</th><td>{invoice.TaxAmount:C}</td></tr>
            <tr><th>Total:</th><td>{invoice.TotalAmount:C}</td></tr>
            <tr><th>Balance due:</th><td>{invoice.BalanceDue:C}</td></tr>
            </table>
            </div>
            </body>
            </html>
            "; return html;}
        public void OpenInvoiceInBrowser(string filePath)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
    }
}