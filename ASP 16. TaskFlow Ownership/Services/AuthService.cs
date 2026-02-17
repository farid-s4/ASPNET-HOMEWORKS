using ASP_16._TaskFlow_Ownership.Data;
using ASP_16._TaskFlow_Ownership.DTOs.Auth_DTOs;
using ASP_16._TaskFlow_Ownership.Models;
using ASP_16._TaskFlow_Ownership.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP_16._TaskFlow_Ownership.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly RoleManager<IdentityUser> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly TaskFlowDbContext _context;

    private const string RefreshTokenType = "refresh";

    public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            TaskFlowDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
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

        return await GenerateTokenAsync(user);

    }


    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
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

        // Admin, Manager, User
        await _userManager.AddToRoleAsync(user, "User");

        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        var (principal, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken);

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);

        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if(!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId!);

        if(user is null)
            throw new UnauthorizedAccessException("User not found");

        storedToken.RevokedAt = DateTime.UtcNow;

        var newToken = await GenerateTokenAsync(user);
        var newStored = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.JwtId == GetJtiFromRefreshToken(newToken.RefreshToken));

        if (newStored is not null) storedToken.ReplacedByJwtId = newStored.JwtId;

        await _context.SaveChangesAsync();
        return newToken;

    }
    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        string? jti;
        try
        {
            (_, jti) = ValidateRefreshJwtAndGetJti(refreshToken, validateLifeTime: false);
        }
        catch
        {
            return;
        }
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);
        
        if (storedToken is null || !storedToken.IsActive) return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]!);
        var refreshTokenExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationInDays"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
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
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
            );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var (refreshToken, jwt) = await CreateRefreshTokenJwtAsync(user.Id, refreshTokenExpirationDays);

        return new AuthResponseDto
        {
            Email = user.Email!,
            AccessToken = tokenString,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            RefreshToken = jwt,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            Roles = roles
        };

    }

    private async Task<(RefreshToken entity, string jwt)> CreateRefreshTokenJwtAsync(string userId, int expirationDays)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var refreshSecretKey = jwtSettings["RefreshTokenSecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var jti = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshSecretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("token_type", RefreshTokenType)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
            );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        var entity = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();

        return (entity, jwtString);

    }

    private (ClaimsPrincipal principal, string jti)
        ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifeTime = true)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var refreshSecretKey = jwtSettings["RefreshTokenSecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshSecretKey!));

        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = validateLifeTime,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var tokenType = jwt.Claims.FirstOrDefault(x => x.Type == "token_type")?.Value;

        if (tokenType != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var jti = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value
                        ?? throw new UnauthorizedAccessException("Invalid refresh token");

        return (principal, jti);


    }
    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(refreshJwt)) return string.Empty;

        var jwt = handler.ReadJwtToken(refreshJwt);
        return jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }
}
