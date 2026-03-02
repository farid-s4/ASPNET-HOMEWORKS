using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthUserStore _authUserStore;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IAuthUserStore authUserStore, ITokenService tokenService)
    {
        _authUserStore = authUserStore;
        _tokenService = tokenService;
    }
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userId = await _authUserStore.FindUserIdByEmailOrIdAsync(request.RequestDto.Email);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!await _authUserStore.CheckPasswordAsync(userId, request.RequestDto.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await _tokenService.GenerateToken(userId, request.RequestDto.Email);
    }
}