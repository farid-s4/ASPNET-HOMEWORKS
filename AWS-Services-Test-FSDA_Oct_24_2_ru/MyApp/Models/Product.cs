using System.ComponentModel.DataAnnotations;

namespace MyApp.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Description { get; set; }
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    [Range(0.01, 100000)]
    public decimal CurrentPrice { get; set; }
    public decimal BasePrice { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? DiscountStart { get; set; }
    public DateTime? DiscountEnd { get; set; }
    public bool IsDiscountActive { get; set; }
    public DateTime CreatedAt { get; set; }

}
