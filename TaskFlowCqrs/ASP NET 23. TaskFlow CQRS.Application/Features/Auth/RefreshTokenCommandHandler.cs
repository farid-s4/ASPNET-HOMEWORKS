using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuthUserStore _authUserStore;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IAuthUserStore authUserStore, ITokenService tokenService)
    {
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _authUserStore = authUserStore;
        _tokenService = tokenService;
    }
    
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var (userId, jti) = _jwtTokenService.ValidateRefreshTokenAndGetJti(request.RefreshTokenRequest.RefreshToken);

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
}