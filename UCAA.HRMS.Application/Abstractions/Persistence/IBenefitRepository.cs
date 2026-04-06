using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IBenefitRepository
{
    Task<List<BenefitPlan>> ListPlansAsync(CancellationToken ct = default);
    Task<BenefitPlan?> GetPlanByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> PlanNameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddPlanAsync(BenefitPlan plan, CancellationToken ct = default);
    void UpdatePlan(BenefitPlan plan);

    Task<List<BenefitEnrollment>> ListEnrollmentsAsync(CancellationToken ct = default);
    Task<BenefitEnrollment?> GetEnrollmentByIdAsync(Guid id, CancellationToken ct = default);
    Task AddEnrollmentAsync(BenefitEnrollment enrollment, CancellationToken ct = default);
    void UpdateEnrollment(BenefitEnrollment enrollment);
}
