using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthUserStore _authUserStore;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public AuthService(
        IAuthUserStore authUserStore,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _authUserStore = authUserStore;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var userId = await _authUserStore.FindUserIdByEmailOrIdAsync(loginRequest.Email);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!await _authUserStore.CheckPasswordAsync(userId, loginRequest.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await _tokenService.GenerateToken(userId, loginRequest.Email);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        if (await _authUserStore.FindUserIdByEmailOrIdAsync(registerRequest.Email) is not null)
            throw new InvalidOperationException("User with this email already exists.");

        var userId = await _authUserStore.CreateUserAsync(registerRequest);
        await _authUserStore.AddToRoleAsync(userId, "User");

        return await _tokenService.GenerateToken(userId, registerRequest.Email);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var (userId, jti) = _jwtTokenService.ValidateRefreshTokenAndGetJti(refreshTokenRequest.RefreshToken);

        var storedToken = await _refreshTokenRepository.GetByJwtIdAsync(jti);
        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");
        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");

        storedToken.RevokedAt = DateTime.UtcNow;

        var email = await _authUserStore.GetEmailAsync(userId);
        var newTokens = await _tokenService.GenerateToken(userId, email);
        var newJti = _jwtTokenService.GetJtiFromRefreshToken(newTokens.RefreshToken);
        var newStoredToken = string.IsNullOrEmpty(newJti) ? null : await _refreshTokenRepository.GetByJwtIdAsync(newJti);
        if (newStoredToken is not null)
            storedToken.ReplacedByJwtId = newStoredToken.JwtId;

        await _refreshTokenRepository.UpdateAsync(storedToken);
        return newTokens;
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        string jti;
        try
        {
            (_, jti) = _jwtTokenService.ValidateRefreshTokenAndGetJti(refreshTokenRequest.RefreshToken, validateLifetime: false);
        }
        catch
        {
            return;
        }

        var storedToken = await _refreshTokenRepository.GetByJwtIdAsync(jti);
        if (storedToken is null || !storedToken.IsActive) return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken);
    }
    
}
