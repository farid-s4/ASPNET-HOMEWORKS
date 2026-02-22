namespace InvoiceManager.DTO.CustomerDTOs;

public class CustomerQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
    public string? Search { get; set; }
    public int? InvoiceId { get; set; }
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