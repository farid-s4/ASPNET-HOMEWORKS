namespace InvoiceManager.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }= string.Empty;
        public string Email { get; set; }= string.Empty;
        public string? Phone { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        
        public ApplicationUser? User { get; set; } = null;
        public string? UserId { get; set; } = null;

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
