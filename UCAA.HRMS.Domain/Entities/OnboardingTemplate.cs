using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

/// <summary>Reusable checklist template — defines a standard set of onboarding tasks.</summary>
public sealed class OnboardingTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<OnboardingTemplateTask> Tasks { get; set; } = new List<OnboardingTemplateTask>();
}
