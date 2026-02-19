using StockService.Models;

namespace StockService.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByCodeAsync(string code);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> UpdateStockAsync(int id, decimal quantity);
    Task<bool> DeleteAsync(int id);
}
