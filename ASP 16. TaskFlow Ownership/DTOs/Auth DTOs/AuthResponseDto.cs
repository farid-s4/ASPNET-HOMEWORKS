namespace ASP_16._TaskFlow_Ownership.DTOs.Auth_DTOs;

public class AuthResponseDto
{
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
