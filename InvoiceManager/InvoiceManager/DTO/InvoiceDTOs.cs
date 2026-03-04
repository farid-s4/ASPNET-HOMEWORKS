using InvoiceManager.Models;

namespace InvoiceManager.DTO;

public class CreateInvoiceDTO
{
    public int CustomerId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string Comment { get; set; } = string.Empty;
    public ICollection<CreateInvoiceRowDTO> InvoiceRows { get; set; } = new List<CreateInvoiceRowDTO>();
}
public class CreateInvoiceRowDTO
{
    public string Service { get; set; } = string.Empty; // Название выполненной работы
    public decimal Quantity { get; set; } // Количество единиц выполненной работы
    public decimal Rate { get; set; }
}
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
public class InvoicesQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
    public string? Search { get; set; }
    /// <summary>
    /// Filter tasks by priority (e.g. Draft,Created,Sent,Received,Paid,Cancelled,Rejected///
    /// </summary>
    public string? SortByStatus  { get; set; }
    public int? CustomerId { get; set; }
    
    public void Validate()
    {
        if (PageNumber < 1) PageNumber = 1;

        if (PageSize < 1) PageSize = 10;

        if (PageSize > 100) PageSize = 100;

        if (string.IsNullOrWhiteSpace(SortDirection)) 
            SortDirection = "asc";
        
        SortDirection = SortDirection.ToLower();

        if (SortDirection != "asc" && SortDirection != "desc") 
            SortDirection = "asc";
    }
}
public class InvoiceUpdateDTO
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string Comment { get; set; } = string.Empty;
    public ICollection<InvoiceRow> InvoiceRows { get; set; } = new List<InvoiceRow>();
}
public class InvoiceFileDTO
{
    public byte[] FileBytes { get; set; }
    public string FileName { get; set; }
}