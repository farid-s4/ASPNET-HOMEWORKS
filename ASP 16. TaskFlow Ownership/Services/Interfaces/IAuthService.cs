using ASP_16._TaskFlow_Ownership.DTOs.Auth_DTOs;

namespace ASP_16._TaskFlow_Ownership.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
