using MyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if(await context.Products.AnyAsync())
        {
            return;
        }

        var products = new List<Product>
{
    new()
    {
        Name = "Wireless Mouse",
        Description = "Ergonomic wireless mouse with USB receiver",
        BasePrice = 23.12m,
        CurrentPrice = 23.12m,
        Category = "Electronics",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Mechanical Keyboard",
        Description = "RGB backlit mechanical gaming keyboard",
        BasePrice = 59.99m,
        CurrentPrice = 59.99m,
        Category = "Electronics",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Laptop Stand",
        Description = "Adjustable aluminum stand for laptops",
        BasePrice = 34.50m,
        CurrentPrice = 34.50m,
        Category = "Accessories",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "USB-C Hub",
        Description = "Multiport adapter with HDMI and USB 3.0",
        BasePrice = 42.75m,
        CurrentPrice  = 42.75m,
        Category = "Accessories",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Gaming Headset",
        Description = "Surround sound headset with microphone",
        BasePrice = 68.20m,
        CurrentPrice =68.20m,
        Category = "Audio",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
};
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
