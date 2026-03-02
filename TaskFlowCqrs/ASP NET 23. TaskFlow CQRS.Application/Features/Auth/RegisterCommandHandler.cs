using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAuthUserStore _authUserStore;
    private readonly ITokenService _tokenService;
    public RegisterCommandHandler(IAuthUserStore authUserStore, ITokenService tokenService)
    {
        _authUserStore = authUserStore;
        _tokenService = tokenService;
    }
    public async Task<AuthResponseDto> Handle(RegisterCommand registerRequest, CancellationToken cancellationToken)
    {
        if (await _authUserStore.FindUserIdByEmailOrIdAsync(registerRequest.RequestDto.Email) is not null)
            throw new InvalidOperationException("User with this email already exists.");

        var userId = await _authUserStore.CreateUserAsync(registerRequest.RequestDto);
        await _authUserStore.AddToRoleAsync(userId, "User");

        return await _tokenService.GenerateToken(userId, registerRequest.RequestDto.Email);
    }
}