using System.ComponentModel.DataAnnotations;

namespace MyApp.DTOs;

public class ProductUpdateDto
{
    public string Name { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Description { get; set; }
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    [Range(0.01, 100000)]
    public decimal Price { get; set; }
    public IFormFile? Image { get; set; }
    public DateTime? DiscountStart { get; set; }
    public DateTime? DiscountEnd { get; set; }

}
