using AWS_Services_Test_FSDA_Oct_24_2_ru.Models;
using Microsoft.EntityFrameworkCore;

namespace AWS_Services_Test_FSDA_Oct_24_2_ru.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) 
        : base(options)
    {}
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(
            entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(10, 2)");
            });

        modelBuilder.Entity<Product>().HasIndex(p => new { p.IsDiscountActive, p.DiscountStart, p.DiscountEnd })
            .HasDatabaseName("Prosucts_IsDiscountActive_DiscountStart_DiscountEnd");
    }

}
