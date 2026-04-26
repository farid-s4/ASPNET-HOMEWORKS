using System.ComponentModel.DataAnnotations;

namespace AWS_Services_Test_FSDA_Oct_24_2_ru.Models;

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
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? DiscountStart { get; set; }
    public DateTime? DiscountEnd { get; set; }
    public bool IsDiscountActive { get; set; }
    public DateTime CreatedAt { get; set; }

}
