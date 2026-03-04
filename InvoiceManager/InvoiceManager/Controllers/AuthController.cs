using System.Security.Claims;
using InvoiceManager.Common;
using InvoiceManager.DTO;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private IAuthService  _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody]RegisterUserDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResult(result, "User registered successfully"));
    }
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody]LoginUserDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResult(result, "Login successfully"));
    }
    [Authorize]
    [HttpPost("reset-password")]
    
    public async Task<ActionResult> ResetPassword([FromBody]ChangePasswordDto passwordDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _authService.ChangePasswordAsync(passwordDto, userId!);

        return Ok(ApiResponse<object>.SuccessResult(null, "Password changed successfully"));
    }

    [Authorize]
    [HttpPost("change-profile")]
    public async Task<ActionResult> ChangeProfile([FromBody] ChangeProfileDataDto profileDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _authService.ChangeProfileAsync(profileDto, userId!);
        return Ok(ApiResponse<object>.SuccessResult(null, "Profile data changed successfully"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto refreshTokenRequest)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResult(result, "Token refreshed successfully"));
    }

    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequestDto refreshTokenRequest)
    {
        await _authService.RevokeRefreshTokenAsync(refreshTokenRequest);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResult(null,"Refresh token revoked"));
    }
}