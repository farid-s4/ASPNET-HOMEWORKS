using MyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Data;

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
                entity.Property(p => p.BasePrice).HasColumnType("decimal(10, 2)");
                entity.Property(p=>p.CurrentPrice).HasColumnType("decimal(10, 2)");
            });

        modelBuilder.Entity<Product>().HasIndex(p => new { p.IsDiscountActive, p.DiscountStart, p.DiscountEnd })
            .HasDatabaseName("Prosucts_IsDiscountActive_DiscountStart_DiscountEnd");
    }

}
