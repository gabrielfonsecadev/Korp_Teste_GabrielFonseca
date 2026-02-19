using BillingService.Models;

namespace BillingService.Repositories;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(int id);
    Task<Invoice?> GetByNumberAsync(int number);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<Invoice> UpdateAsync(Invoice invoice);
    Task<int> GetNextInvoiceNumberAsync();
    Task DeleteAsync(Invoice invoice);
}
