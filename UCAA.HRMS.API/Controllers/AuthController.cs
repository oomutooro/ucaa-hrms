using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.Abstractions.Auth;
using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthProvider _authProvider;

    public AuthController(IAuthProvider authProvider)
    {
        _authProvider = authProvider;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authProvider.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authProvider.RegisterAsync(request, cancellationToken);
        return Ok(response);
    }
}
