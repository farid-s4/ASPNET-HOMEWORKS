namespace InvoiceManager.DTO.AuthDTOs;

public class RegisterUserDto
{
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; } =  string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}