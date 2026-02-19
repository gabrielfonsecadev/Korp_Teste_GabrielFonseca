using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Invoice
{
    public int Id { get; set; }

    [Required]
    public int Number { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? PrintedAt { get; set; }

    public List<InvoiceItem> Items { get; set; } = new();
}
