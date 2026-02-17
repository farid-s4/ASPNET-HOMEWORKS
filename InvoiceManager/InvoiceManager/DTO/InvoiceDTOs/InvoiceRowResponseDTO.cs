using InvoiceManager.Models;

namespace InvoiceManager.DTO.InvoiceDTOs
{
    public class InvoiceRowResponseDTO
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceName { get; set; } = string.Empty;

        public string Service { get; set; } = string.Empty; // Название выполненной работы
        public decimal Quantity { get; set; } // Количество единиц выполненной работы
        public decimal Rate { get; set; } // стоимость одной единицы 
        public decimal Amount { get; set; } // Общая стоимость (Quantity * Rate)
    }
}
