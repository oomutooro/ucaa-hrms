using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Abstractions.Auth;

public interface IAuthProvider
{
    string Name { get; }
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
