using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RevokeTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }
    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        string jti;
        try
        {
            (_, jti) = _jwtTokenService.ValidateRefreshTokenAndGetJti(request.RefreshTokenRequest.RefreshToken, validateLifetime: false);
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