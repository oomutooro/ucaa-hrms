using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

/// <summary>A single task definition within an onboarding template.</summary>
public sealed class OnboardingTemplateTask : BaseEntity
{
    public Guid TemplateId { get; set; }
    public OnboardingTemplate? Template { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;   // e.g. "IT Setup", "HR Admin", "Documents"
    public bool IsRequired { get; set; } = true;
    public int SortOrder { get; set; }
}
