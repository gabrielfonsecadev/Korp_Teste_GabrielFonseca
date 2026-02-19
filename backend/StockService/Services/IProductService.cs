using StockService.Models;

namespace StockService.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(int id, Product product);
    Task<bool> UpdateProductStockAsync(int id, decimal quantity);
    Task<bool> DeleteProductAsync(int id);
}
