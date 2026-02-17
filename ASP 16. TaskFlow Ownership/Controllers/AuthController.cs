using ASP_16._TaskFlow_Ownership.Common;
using ASP_16._TaskFlow_Ownership.DTOs.Auth_DTOs;
using ASP_16._TaskFlow_Ownership.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_16._TaskFlow_Ownership.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody]RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully"));
    }
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody]LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successfully"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto refreshTokenRequest)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully"));
    }

    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke([FromBody] string refreshToken)
    {
        await _authService.RevokeRefreshTokenAsync(refreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse("Refresh token revoked"));
    }
}
