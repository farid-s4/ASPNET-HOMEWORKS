using AutoMapper;
using InvoiceManager.Data;
using InvoiceManager.DTO;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }
        
        await _userManager.AddToRoleAsync(user, "User");
        
        return await _tokenService.GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        return await _tokenService.GenerateTokenAsync(user);
    }

    public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }
        await _tokenService.RevokeAllUserTokensAsync(user.Id);
        await _userManager.UpdateSecurityStampAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new UnauthorizedAccessException("Refresh token is required");

        return await _tokenService.RefreshTokenAsync(request.RefreshToken);
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return;

        await _tokenService.RevokeAsync(request.RefreshToken);
    }

    public async Task ChangeProfileAsync(ChangeProfileDataDto profileDto, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, profileDto.Password);
        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Invalid password");

        if (profileDto.NewFirstName != null)
        {
            user.FirstName = profileDto.NewFirstName;
        }

        if (profileDto.NewLastName != null)
        {
            user.LastName = profileDto.NewLastName;
        }

        if (profileDto.NewEmail != null)
        {
            user.Email = profileDto.NewEmail;
        }

        if (profileDto.NewAddress != null)
        {
            user.Adress = profileDto.NewAddress;
        }
        var result =  await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }
    }
}