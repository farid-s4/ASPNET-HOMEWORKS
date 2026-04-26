using AWS_Services_Test_FSDA_Oct_24_2_ru.Data;
using AWS_Services_Test_FSDA_Oct_24_2_ru.DTOs;
using AWS_Services_Test_FSDA_Oct_24_2_ru.Models;
using AWS_Services_Test_FSDA_Oct_24_2_ru.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AWS_Services_Test_FSDA_Oct_24_2_ru.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IStorage _storage;

    public ProductsController(ApplicationDbContext context, IStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts()
    {
        var products = await _context.Products
                                    .OrderByDescending(p => p.CreatedAt)
                                    .Select(p => MapToReadDto(p))
                                    .ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductReadDto>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null) return NotFound("Product not found");

        return Ok(MapToReadDto(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductReadDto>> CreateProduct([FromForm] ProductCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        ValidateDiscountRange(dto.DiscountStart, dto.DiscountEnd);

        var imageUrl = await _storage.UploadFileAsync(dto.Image);

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Category = dto.Category,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            DiscountStart = dto.DiscountStart,
            DiscountEnd = dto.DiscountEnd,
            IsDiscountActive = false
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, MapToReadDto(product));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductReadDto>> UpdateProduct(int id, [FromForm] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        ValidateDiscountRange(dto.DiscountStart, dto.DiscountEnd);

        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound("Product not found");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Category = dto.Category;
        product.DiscountStart = dto.DiscountStart;
        product.DiscountEnd = dto.DiscountEnd;

        if (dto.Image != null)
        {
            var imageUrl = await _storage.UploadFileAsync(dto.Image);
            product.ImageUrl = imageUrl;
        }

        await _context.SaveChangesAsync();
        return Ok(MapToReadDto(product));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound("Product not found");

        await _storage.DeleteFileByUrl(product.ImageUrl);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private void ValidateDiscountRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue && start > end)
        {
            ModelState.AddModelError("DiscountEnd", "Discount end date must be after start date.");
            return;
        }
        if(start.HasValue != end.HasValue)
        {
            ModelState.AddModelError("DiscountRange", "Both DiscountStart and DiscountEnd must be set together.");
            return;
        }
    }

    private static ProductReadDto MapToReadDto(Product p)
    {
        return new ProductReadDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Category = p.Category,
            ImageUrl = p.ImageUrl,
            CreatedAt = p.CreatedAt,
            DiscountStart = p.DiscountStart,
            DiscountEnd = p.DiscountEnd,
            IsDiscountActive = p.IsDiscountActive
        };
    }
}
