using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class BenefitEnrollment : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public Guid BenefitPlanId { get; set; }
    public BenefitPlan? BenefitPlan { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public BenefitEnrollmentStatus Status { get; set; } = BenefitEnrollmentStatus.Active;
    public decimal EmployerContribution { get; set; }
    public decimal EmployeeContribution { get; set; }
    public string Notes { get; set; } = string.Empty;
}
