using Microsoft.AspNetCore.Identity;

namespace InvoiceManager.Models;

public class ApplicationUser : IdentityUser
{
    public string Adress { get; set; } =  string.Empty;
    public string FirstName { get; set; } =  string.Empty;
    public string LastName { get; set; } =  string.Empty;
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset RefreshTokenExpiry { get; set; }
    
    public ICollection<Customer> Customers  { get; set; } = new List<Customer>();
}