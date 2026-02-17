namespace ASP_16._TaskFlow_Ownership.DTOs.Auth_DTOs;

public class UserWithRolesDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
