using Microsoft.AspNetCore.Identity;

namespace UCAA.HRMS.Infrastructure.Auth;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
}
