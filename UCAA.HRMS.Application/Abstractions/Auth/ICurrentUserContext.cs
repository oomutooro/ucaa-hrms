namespace UCAA.HRMS.Application.Abstractions.Auth;

public interface ICurrentUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsInRole(string role);
}
