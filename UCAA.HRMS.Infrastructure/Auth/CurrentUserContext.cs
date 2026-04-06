using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UCAA.HRMS.Application.Abstractions.Auth;

namespace UCAA.HRMS.Infrastructure.Auth;

public sealed class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User.IsInRole(role) == true;
}
