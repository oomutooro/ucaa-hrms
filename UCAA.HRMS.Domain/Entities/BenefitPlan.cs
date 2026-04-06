using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class BenefitPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public BenefitPlanType PlanType { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsTaxable { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal DefaultEmployerContribution { get; set; }
    public decimal DefaultEmployeeContribution { get; set; }
    public ICollection<BenefitEnrollment> Enrollments { get; set; } = new List<BenefitEnrollment>();
}
