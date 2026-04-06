using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

/// <summary>An individual checklist item within an employee's onboarding record.</summary>
public sealed class OnboardingItem : BaseEntity
{
    public Guid OnboardingId { get; set; }
    public EmployeeOnboarding? Onboarding { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}
