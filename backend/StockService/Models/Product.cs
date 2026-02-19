using System.ComponentModel.DataAnnotations;

namespace StockService.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O código é obrigatório")]
    [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "O saldo deve ser maior ou igual a zero")]
    public decimal Stock { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
