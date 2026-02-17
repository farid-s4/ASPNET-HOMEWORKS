using InvoiceManager.Models;

namespace InvoiceManager.DTO.InvoiceDTOs
{
    public class CreateInvoiceDTO
    {
        public int CustomerId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public ICollection<CreateInvoiceRowDTO> InvoiceRows { get; set; } = new List<CreateInvoiceRowDTO>();
    }
}
