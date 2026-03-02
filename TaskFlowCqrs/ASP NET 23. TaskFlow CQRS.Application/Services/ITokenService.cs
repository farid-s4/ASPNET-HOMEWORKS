using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Services;

public interface ITokenService
{
    Task<AuthResponseDto> GenerateToken(string userId, string? email);
}