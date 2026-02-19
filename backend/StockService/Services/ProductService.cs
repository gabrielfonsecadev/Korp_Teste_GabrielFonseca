using StockService.Models;
using StockService.Repositories;

namespace StockService.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        // Validate unique code
        var existingProduct = await _repository.GetByCodeAsync(product.Code);
        if (existingProduct != null)
        {
            throw new InvalidOperationException($"Já existe um produto com o código '{product.Code}'");
        }

        // Validate stock
        if (product.Stock < 0)
        {
            throw new InvalidOperationException("O saldo não pode ser negativo");
        }

        return await _repository.CreateAsync(product);
    }

    public async Task<Product> UpdateProductAsync(int id, Product product)
    {
        var existingProduct = await _repository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
        }

        // Validate unique code (if changed)
        if (existingProduct.Code != product.Code)
        {
            var duplicateProduct = await _repository.GetByCodeAsync(product.Code);
            if (duplicateProduct != null)
            {
                throw new InvalidOperationException($"Já existe um produto com o código '{product.Code}'");
            }
        }

        // Validate stock
        if (product.Stock < 0)
        {
            throw new InvalidOperationException("O saldo não pode ser negativo");
        }

        existingProduct.Code = product.Code;
        existingProduct.Description = product.Description;
        existingProduct.Stock = product.Stock;

        return await _repository.UpdateAsync(existingProduct);
    }

    public async Task<bool> UpdateProductStockAsync(int id, decimal quantity)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
        }

        // Validate that stock won't go negative
        if (product.Stock + quantity < 0)
        {
            throw new InvalidOperationException($"Saldo insuficiente. Saldo atual: {product.Stock}, Quantidade solicitada: {Math.Abs(quantity)}");
        }

        return await _repository.UpdateStockAsync(id, quantity);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
        }

        return await _repository.DeleteAsync(id);
    }
}
