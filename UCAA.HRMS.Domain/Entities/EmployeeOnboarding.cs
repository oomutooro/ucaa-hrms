using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

/// <summary>Onboarding record for a single employee — holds their personalised checklist.</summary>
public sealed class EmployeeOnboarding : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public Guid? ApplicationId { get; set; }   // optionally linked to the hiring application
    public DateOnly StartDate { get; set; }
    public DateOnly TargetCompletionDate { get; set; }
    public OnboardingStatus Status { get; set; } = OnboardingStatus.NotStarted;
    public string? Notes { get; set; }
    public ICollection<OnboardingItem> Items { get; set; } = new List<OnboardingItem>();
}
