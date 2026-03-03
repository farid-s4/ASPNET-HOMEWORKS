using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManager.Models
{
    public class InvoiceRow
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;

        public string Service { get; set; } = string.Empty; // Название выполненной работы
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } // Количество единиц выполненной работы
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; } // стоимость одной единицы 
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
    }
}
