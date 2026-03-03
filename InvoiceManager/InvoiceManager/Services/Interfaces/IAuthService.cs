using InvoiceManager.DTO.AuthDTOs;

namespace InvoiceManager.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginUserDto loginDto);
    Task ChangePasswordAsync(ChangePasswordDto changePasswordDto, string userId);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
    Task RevokeRefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
    Task ChangeProfileAsync(ChangeProfileDataDto profileDto, string userId);
}