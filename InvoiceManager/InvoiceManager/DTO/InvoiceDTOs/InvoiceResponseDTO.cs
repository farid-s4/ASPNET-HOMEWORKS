using InvoiceManager.Models;

namespace InvoiceManager.DTO.InvoiceDTOs
{
    public class InvoiceResponseDTO
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public ICollection<InvoiceRowResponseDTO> InvoiceRows { get; set; } = new List<InvoiceRowResponseDTO>();
        public decimal TotalAmount { get; set; }
        public string Comment { get; set; } = string.Empty;
        public InvoiceStatus Status { get; set; } 
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
