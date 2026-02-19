namespace BillingService.Services;

public interface IStockServiceClient
{
    Task<ProductDto?> GetProductAsync(int productId);
    Task<bool> UpdateStockAsync(int productId, decimal quantity);
}
