using InvoiceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(x => x.Invoices)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<Invoice>()
                .HasMany(x => x.InvoiceRows)
                .WithOne(x => x.Invoice)
                .HasForeignKey(x => x.InvoiceId);    
            
        }
    }
}
