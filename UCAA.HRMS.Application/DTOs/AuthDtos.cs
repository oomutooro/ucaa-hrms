namespace UCAA.HRMS.Application.DTOs;

public sealed record LoginRequest(string Email, string Password);

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    string Role,
    string EmployeeId);

public sealed record AuthResponse(string AccessToken, string Email, string Role);
