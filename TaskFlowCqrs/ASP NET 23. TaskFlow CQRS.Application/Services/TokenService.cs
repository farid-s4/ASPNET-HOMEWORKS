using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Services;

public class TokenService : ITokenService
{
    private readonly IAuthUserStore _authUserStore;
    private readonly IJwtTokenService _jwtTokenService;
    public TokenService(IAuthUserStore authUserStore, IJwtTokenService jwtTokenService)
    {
        _authUserStore = authUserStore;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<AuthResponseDto> GenerateToken(string userId, string? email)
    {
        var roles = await _authUserStore.GetRolesAsync(userId);
        var (accessToken, expiresAt) = await _jwtTokenService.GenerateAccessTokenAsync(userId, email ?? "", roles);
        var (refreshEntity, refreshJwt) = await _jwtTokenService.CreateRefreshTokenAsync(userId);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshJwt,
            RefreshTokenExpiresAt = refreshEntity.ExpiresAt,
            Email = email ?? "",
            Roles = roles
        };
    }
}