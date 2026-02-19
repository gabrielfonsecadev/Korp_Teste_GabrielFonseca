namespace BillingService.Services;

public class ProductDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Stock { get; set; }
}

public class UpdateStockRequest
{
    public decimal Quantity { get; set; }
}
