using Microsoft.AspNetCore.Mvc;
using BillingService.Models;
using BillingService.Services;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IInvoiceService service, ILogger<InvoicesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as notas fiscais
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
    {
        _logger.LogInformation("Buscando todas as notas fiscais");
        var invoices = await _service.GetAllInvoicesAsync();
        return Ok(invoices);
    }

    /// <summary>
    /// Retorna uma nota fiscal por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Invoice>> GetById(int id)
    {
        _logger.LogInformation("Buscando nota fiscal com ID {InvoiceId}", id);
        var invoice = await _service.GetInvoiceByIdAsync(id);
        
        if (invoice == null)
        {
            return NotFound(new { error = $"Nota fiscal com ID {id} não encontrada" });
        }

        return Ok(invoice);
    }

    /// <summary>
    /// Cria uma nova nota fiscal
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Invoice>> Create([FromBody] CreateInvoiceRequest request)
    {
        _logger.LogInformation("Criando nova nota fiscal com {ItemCount} itens", request.Items?.Count ?? 0);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var invoice = await _service.CreateInvoiceAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    /// <summary>
    /// Imprime uma nota fiscal (atualiza estoque e fecha a nota)
    /// </summary>
    [HttpPost("{id}/print")]
    public async Task<ActionResult<Invoice>> Print(int id)
    {
        _logger.LogInformation("Solicitação de impressão para nota fiscal {InvoiceId}", id);
        
        var invoice = await _service.PrintInvoiceAsync(id);
        return Ok(invoice);
    }

    /// <summary>
    /// Exclui uma nota fiscal (apenas status Open)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Solicitação de exclusão para nota fiscal {InvoiceId}", id);
        
        await _service.DeleteInvoiceAsync(id);
        return NoContent();
    }
}
