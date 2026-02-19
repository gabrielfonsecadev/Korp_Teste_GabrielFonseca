using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class InvoiceItem
{
    public int Id { get; set; }

    [Required]
    public int InvoiceId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [StringLength(50)]
    public string ProductCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string ProductDescription { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public decimal Quantity { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
