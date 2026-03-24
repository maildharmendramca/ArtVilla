using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Exceptions;
using IndianArtVilla.Core.Interfaces.Repositories;
using IndianArtVilla.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IndianArtVilla.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _config = config;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new BusinessRuleViolationException("An account with this email already exists.");

        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email,
            Phone = dto.Phone,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Customer");

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedDomainException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedDomainException("Account is deactivated.");

        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!validPassword)
            throw new UnauthorizedDomainException("Invalid email or password.");

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var principal = GetPrincipalFromExpiredToken(dto.Token);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedDomainException("Invalid token.");

        // Validate the refresh token from the database
        var storedToken = await _unitOfWork.RefreshTokens.Query()
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && rt.UserId == userId);

        if (storedToken == null || !storedToken.IsActive)
            throw new UnauthorizedDomainException("Invalid or expired refresh token.");

        // Revoke old refresh token
        storedToken.RevokedAt = DateTime.UtcNow;

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedDomainException("User not found.");

        var response = await GenerateAuthResponse(user);
        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    public Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        return Task.CompletedTask;
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new NotFoundException("User", dto.Email);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            throw new BusinessRuleViolationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<UserDto> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Phone = user.Phone,
            Roles = roles.ToList()
        };
    }

    public async Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        if (!string.IsNullOrWhiteSpace(dto.FullName))
            user.FullName = dto.FullName;
        if (dto.Phone is not null)
            user.Phone = dto.Phone;

        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Phone = user.Phone,
            Roles = roles.ToList()
        };
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<PagedResult<CustomerDto>> GetCustomersAsync(int page, int pageSize)
    {
        var customersInRole = await _userManager.GetUsersInRoleAsync("Customer");
        var allCustomers = customersInRole.ToList();
        var total = allCustomers.Count;

        var customers = new List<CustomerDto>();
        foreach (var u in allCustomers.Skip((page - 1) * pageSize).Take(pageSize))
        {
            var orderCount = await _unitOfWork.Orders.CountAsync(o => o.UserId == u.Id);
            customers.Add(new CustomerDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email!,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive,
                OrderCount = orderCount
            });
        }

        return new PagedResult<CustomerDto>
        {
            Items = customers,
            Pagination = new PaginationMeta
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            }
        };
    }

    public async Task<CustomerDto?> GetCustomerAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;

        var orderCount = await _unitOfWork.Orders.CountAsync(o => o.UserId == user.Id);
        return new CustomerDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Phone = user.Phone,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            OrderCount = orderCount
        };
    }

    // ── Private helpers ──────────────────────────────────────────

    private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        // Persist the refresh token
        _unitOfWork.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(
                double.Parse(_config["Jwt:ExpirationInMinutes"] ?? "1440")),
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Phone = user.Phone,
                Roles = roles.ToList()
            }
        };
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpirationInMinutes"] ?? "1440")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!)),
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
