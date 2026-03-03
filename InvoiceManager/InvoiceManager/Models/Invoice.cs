using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManager.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public ICollection<InvoiceRow> InvoiceRows { get; set; } = new List<InvoiceRow>();
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public string Comment { get; set; } = string.Empty;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft; // Possible values: Created, Sent, Received, Paid, Cancelled, Rejected
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

    }
}
