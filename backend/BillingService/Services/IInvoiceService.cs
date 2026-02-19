using BillingService.Models;

namespace BillingService.Services;

public class CreateInvoiceRequest
{
    public List<CreateInvoiceItemRequest> Items { get; set; } = new();
}

public class CreateInvoiceItemRequest
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}

public interface IInvoiceService
{
    Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
    Task<Invoice?> GetInvoiceByIdAsync(int id);
    Task<Invoice> CreateInvoiceAsync(CreateInvoiceRequest request);
    Task<Invoice> PrintInvoiceAsync(int id);
    Task DeleteInvoiceAsync(int id);
}
