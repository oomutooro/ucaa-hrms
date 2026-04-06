using UCAA.HRMS.Application.Abstractions.Auth;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Infrastructure.Auth;

// Future extension point for Microsoft Entra ID (Azure AD) using OAuth2/OIDC.
public sealed class MicrosoftAuthProvider : IMicrosoftAuthProvider
{
    public string Name => "Microsoft";

    public Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => throw new AppException("Microsoft authentication provider is not implemented yet.", 501);

    public Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        => throw new AppException("Microsoft authentication provider is not implemented yet.", 501);
}
