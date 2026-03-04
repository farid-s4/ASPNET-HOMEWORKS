namespace InvoiceManager.DTO;

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ChangeProfileDataDto
{
    public string? NewEmail { get; set; } = string.Empty;
    public string? NewAddress { get; set; } =  string.Empty;
    public string? NewFirstName { get; set; } = string.Empty;
    public string? NewLastName { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
}