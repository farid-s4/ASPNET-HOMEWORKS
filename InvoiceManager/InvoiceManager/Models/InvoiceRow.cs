namespace InvoiceManager.Models
{
    public class InvoiceRow
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string Service { get; set; } = string.Empty; // Название выполненной работы
        public decimal Quantity { get; set; } // Количество единиц выполненной работы
        public decimal Rate { get; set; } // стоимость одной единицы 
        public decimal Amount => Rate * Quantity; // Общая стоимость (Quantity * Rate)
    }
}
