using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InvoiceManager.Config;
using InvoiceManager.Data;
using InvoiceManager.DTO.AuthDTOs;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InvoiceManager.Services;

public class TokenService : ITokenService
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwt;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public TokenService(AppDbContext context, UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwt)
    {
        _context = context;
        _userManager = userManager;
        _jwt = jwt.Value;
    }
    public async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
    {
        var accessToken = await GenerateAccessTokenAsync(user);

        var refreshToken = await GenerateAndStoreRefreshTokenAsync(user);

        return new AuthResponseDto
        {
            Email = user.Email!,
            AccessToken = accessToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwt.ExpirationInMinutes),
            RefreshToken = refreshToken.RawToken,
            RefreshTokenExpiresAt = refreshToken.Entity.ExpiresAt
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var token = ComputeHash(refreshToken);
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == token);
        if (storedToken == null)
        {
            throw new UnauthorizedAccessException("Refresh token not found");
        }

        if (storedToken.ExpiresAt < DateTime.UtcNow || storedToken.RevokedAt.HasValue)
        {
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");
        }
        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        
        storedToken.RevokedAt = DateTime.UtcNow;
        
        var newToken = await GenerateTokenAsync(user);
        var newTokenHash = ComputeHash(newToken.RefreshToken);
        storedToken.ReplacedByTokenHash = newTokenHash;
        await _context.SaveChangesAsync();
        return newToken;
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var hash = ComputeHash(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash);

        if (storedToken == null || storedToken.RevokedAt.HasValue)
            return;

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        var tokens = _context.RefreshTokens
            .Where(t => t.UserId == userId
                        && !t.RevokedAt.HasValue
                        && t.ExpiresAt < DateTime.UtcNow)
            .ToList();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
    }

    private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationInMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return  tokenString;
    }
    
    private async Task<(RefreshToken Entity, string RawToken)> GenerateAndStoreRefreshTokenAsync(ApplicationUser user)
    {
        var rawToken = GenerateRefreshToken();
        var hash = ComputeHash(rawToken);

        var entity = new RefreshToken
        {
            TokenHash = hash,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationInDays)
        };

        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();

        return (entity, rawToken);
    }
    
    private string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
    
    private string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}