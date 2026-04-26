using AWS_Services_Test_FSDA_Oct_24_2_ru.Models;
using Microsoft.EntityFrameworkCore;

namespace AWS_Services_Test_FSDA_Oct_24_2_ru.Data;

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
        Price = 23.12m,
        Category = "Electronics",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Mechanical Keyboard",
        Description = "RGB backlit mechanical gaming keyboard",
        Price = 59.99m,
        Category = "Electronics",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Laptop Stand",
        Description = "Adjustable aluminum stand for laptops",
        Price = 34.50m,
        Category = "Accessories",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "USB-C Hub",
        Description = "Multiport adapter with HDMI and USB 3.0",
        Price = 42.75m,
        Category = "Accessories",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Gaming Headset",
        Description = "Surround sound headset with microphone",
        Price = 68.20m,
        Category = "Audio",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Smart Watch",
        Description = "Fitness tracking smart watch with heart monitor",
        Price = 120.00m,
        Category = "Wearables",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Portable SSD",
        Description = "1TB external portable SSD drive",
        Price = 99.99m,
        Category = "Storage",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Bluetooth Speaker",
        Description = "Portable waterproof bluetooth speaker",
        Price = 45.00m,
        Category = "Audio",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Webcam",
        Description = "1080p HD webcam for streaming and meetings",
        Price = 37.80m,
        Category = "Electronics",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    },
    new()
    {
        Name = "Monitor 24 Inch",
        Description = "Full HD IPS monitor with thin bezels",
        Price = 149.99m,
        Category = "Displays",
        ImageUrl = null,
        CreatedAt = DateTime.UtcNow
    }
};
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
