namespace ASP_16._TaskFlow_Ownership.DTOs.Auth_DTOs;

public class LoginDto
{
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
}
