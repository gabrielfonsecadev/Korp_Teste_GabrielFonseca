using Microsoft.AspNetCore.Mvc;
using StockService.Models;
using StockService.Services;

namespace StockService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os produtos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        _logger.LogInformation("Buscando todos os produtos");
        var products = await _service.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Retorna um produto por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        _logger.LogInformation("Buscando produto com ID {ProductId}", id);
        var product = await _service.GetProductByIdAsync(id);
        
        if (product == null)
        {
            return NotFound(new { error = $"Produto com ID {id} não encontrado" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        _logger.LogInformation("Criando novo produto com código {ProductCode}", product.Code);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProduct = await _service.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> Update(int id, [FromBody] Product product)
    {
        _logger.LogInformation("Atualizando produto com ID {ProductId}", id);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedProduct = await _service.UpdateProductAsync(id, product);
        return Ok(updatedProduct);
    }

    /// <summary>
    /// Atualiza o saldo de um produto (usado pelo serviço de faturamento)
    /// </summary>
    [HttpPatch("{id}/stock")]
    public async Task<ActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
    {
        _logger.LogInformation("Atualizando saldo do produto {ProductId} em {Quantity}", id, request.Quantity);
        
        await _service.UpdateProductStockAsync(id, request.Quantity);
        return NoContent();
    }

    /// <summary>
    /// Deleta um produto
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deletando produto com ID {ProductId}", id);
        
        await _service.DeleteProductAsync(id);
        return NoContent();
    }
}

public record UpdateStockRequest(decimal Quantity);
