using InvoiceManager.Models;

namespace InvoiceManager.DTO.InvoiceDTOs
{
    public class InvoiceUpdateDTO
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public ICollection<InvoiceRow> InvoiceRows { get; set; } = new List<InvoiceRow>();
    }
}
