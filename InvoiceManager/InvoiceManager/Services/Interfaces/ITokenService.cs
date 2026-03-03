using InvoiceManager.DTO.AuthDTOs;
using InvoiceManager.Models;

namespace InvoiceManager.Services.Interfaces;

public interface ITokenService
{
    Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
    Task RevokeAllUserTokensAsync(string userId);
}