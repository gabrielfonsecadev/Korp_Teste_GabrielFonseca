using BillingService.Models;
using BillingService.Repositories;

namespace BillingService.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IStockServiceClient _stockClient;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IInvoiceRepository repository,
        IStockServiceClient stockClient,
        ILogger<InvoiceService> logger)
    {
        _repository = repository;
        _stockClient = stockClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Invoice> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            throw new InvalidOperationException("A nota fiscal deve conter pelo menos um item");
        }

        // Validate all products exist and have sufficient stock
        var invoiceItems = new List<InvoiceItem>();
        
        foreach (var item in request.Items)
        {
            var product = await _stockClient.GetProductAsync(item.ProductId);
            
            if (product == null)
            {
                throw new InvalidOperationException($"Produto com ID {item.ProductId} não encontrado");
            }

            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException(
                    $"Saldo insuficiente para o produto '{product.Description}'. " +
                    $"Saldo disponível: {product.Stock}, Quantidade solicitada: {item.Quantity}");
            }

            invoiceItems.Add(new InvoiceItem
            {
                ProductId = product.Id,
                ProductCode = product.Code,
                ProductDescription = product.Description,
                Quantity = item.Quantity
            });
        }

        // Get next invoice number
        var nextNumber = await _repository.GetNextInvoiceNumberAsync();

        // Create invoice
        var invoice = new Invoice
        {
            Number = nextNumber,
            Status = InvoiceStatus.Open,
            Items = invoiceItems
        };

        _logger.LogInformation("Criando nota fiscal número {InvoiceNumber} com {ItemCount} itens", 
            nextNumber, invoiceItems.Count);

        return await _repository.CreateAsync(invoice);
    }

    public async Task<Invoice> PrintInvoiceAsync(int id)
    {
        var invoice = await _repository.GetByIdAsync(id);
        
        if (invoice == null)
        {
            throw new KeyNotFoundException($"Nota fiscal com ID {id} não encontrada");
        }

        if (invoice.Status == InvoiceStatus.Closed)
        {
            throw new InvalidOperationException(
                $"A nota fiscal número {invoice.Number} já foi impressa e não pode ser impressa novamente");
        }

        _logger.LogInformation("Imprimindo nota fiscal número {InvoiceNumber}", invoice.Number);

        // Update stock for all items
        var stockUpdateTasks = invoice.Items.Select(async item =>
        {
            try
            {
                // Negative quantity to decrease stock
                await _stockClient.UpdateStockAsync(item.ProductId, -item.Quantity);
                _logger.LogInformation(
                    "Estoque atualizado para produto {ProductCode}: -{Quantity}", 
                    item.ProductCode, 
                    item.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Erro ao atualizar estoque do produto {ProductCode}", 
                    item.ProductCode);
                throw new InvalidOperationException(
                    $"Falha ao atualizar estoque do produto '{item.ProductDescription}': {ex.Message}", 
                    ex);
            }
        });

        try
        {
            // Execute all stock updates
            await Task.WhenAll(stockUpdateTasks);

            // Update invoice status
            invoice.Status = InvoiceStatus.Closed;
            invoice.PrintedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(invoice);

            _logger.LogInformation("Nota fiscal número {InvoiceNumber} impressa com sucesso", invoice.Number);

            return invoice;
        }
        catch (Exception)
        {
            // If any stock update fails, the invoice remains open
            // In a production system, you might want to implement compensation logic
            _logger.LogError("Falha ao imprimir nota fiscal número {InvoiceNumber}. A nota permanece aberta.", 
                invoice.Number);
            throw;
        }
    }
    public async Task DeleteInvoiceAsync(int id)
    {
        var invoice = await _repository.GetByIdAsync(id);

        if (invoice == null)
        {
            throw new KeyNotFoundException($"Nota fiscal com ID {id} não encontrada");
        }

        if (invoice.Status != InvoiceStatus.Open)
        {
            throw new InvalidOperationException(
                $"A nota fiscal número {invoice.Number} já foi fechada/impressa e não pode ser excluída");
        }

        _logger.LogInformation("Excluindo nota fiscal número {InvoiceNumber}", invoice.Number);
        await _repository.DeleteAsync(invoice);
    }
}
