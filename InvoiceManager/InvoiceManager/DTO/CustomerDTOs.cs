namespace InvoiceManager.DTO;

public class CreateCustomerDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
public class CustomerResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int InvoicesCount { get; set; }
}
public class CustomerUpdateDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

}
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