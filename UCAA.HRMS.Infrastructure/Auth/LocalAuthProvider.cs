using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UCAA.HRMS.Application.Abstractions.Auth;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Infrastructure.Options;

namespace UCAA.HRMS.Infrastructure.Auth;

public sealed class LocalAuthProvider : IAuthProvider
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtOptions _jwtOptions;

    public LocalAuthProvider(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtOptions = jwtOptions.Value;
    }

    public string Name => "Local";

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new AppException("Invalid credentials.", 401);

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            throw new AppException("Invalid credentials.", 401);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Employee";
        return new AuthResponse(GenerateJwt(user, role), user.Email ?? string.Empty, role);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            throw new AppException("Email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmployeeId = request.EmployeeId,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            throw new AppException(string.Join("; ", createResult.Errors.Select(e => e.Description)));
        }

        if (!await _userManager.IsInRoleAsync(user, request.Role))
        {
            var roleAddResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleAddResult.Succeeded)
            {
                throw new AppException(string.Join("; ", roleAddResult.Errors.Select(e => e.Description)));
            }
        }

        return new AuthResponse(GenerateJwt(user, request.Role), user.Email ?? string.Empty, request.Role);
    }

    private string GenerateJwt(ApplicationUser user, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
