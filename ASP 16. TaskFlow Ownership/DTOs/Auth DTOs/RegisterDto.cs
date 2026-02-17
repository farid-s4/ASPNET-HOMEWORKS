namespace ASP_16._TaskFlow_Ownership.DTOs.Auth_DTOs;

public class RegisterDto
{
    /// <summary>
    /// User FirstName
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// User LastName
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ss1234</example>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Confirmed Password
    /// </summary>
    /// <example>P@ss1234</example>
    public string ConfirmPassword { get; set; } = string.Empty;
}
