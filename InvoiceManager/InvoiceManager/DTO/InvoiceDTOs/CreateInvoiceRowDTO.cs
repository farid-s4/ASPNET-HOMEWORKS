namespace InvoiceManager.DTO.InvoiceDTOs
{
    public class CreateInvoiceRowDTO
    {
        public string Service { get; set; } = string.Empty; // Название выполненной работы
        public decimal Quantity { get; set; } // Количество единиц выполненной работы
        public decimal Rate { get; set; }
    }
}
