using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class BenefitRepository : IBenefitRepository
{
    private readonly ApplicationDbContext _db;

    public BenefitRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<BenefitPlan>> ListPlansAsync(CancellationToken ct = default) =>
        _db.BenefitPlans
            .Include(p => p.Enrollments.Where(e => e.Status == Domain.Enums.BenefitEnrollmentStatus.Active))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

    public Task<BenefitPlan?> GetPlanByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.BenefitPlans.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<bool> PlanNameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default) =>
        _db.BenefitPlans.AnyAsync(p => p.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || p.Id != excludeId.Value), ct);

    public Task AddPlanAsync(BenefitPlan plan, CancellationToken ct = default) =>
        _db.BenefitPlans.AddAsync(plan, ct).AsTask();

    public void UpdatePlan(BenefitPlan plan) => _db.BenefitPlans.Update(plan);

    public Task<List<BenefitEnrollment>> ListEnrollmentsAsync(CancellationToken ct = default) =>
        _db.BenefitEnrollments
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.Position)
                    .ThenInclude(p => p!.JobDescription)
                        .ThenInclude(j => j!.JobGrade)
            .Include(e => e.BenefitPlan)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(ct);

    public Task<BenefitEnrollment?> GetEnrollmentByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.BenefitEnrollments
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.Position)
                    .ThenInclude(p => p!.JobDescription)
                        .ThenInclude(j => j!.JobGrade)
            .Include(e => e.BenefitPlan)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task AddEnrollmentAsync(BenefitEnrollment enrollment, CancellationToken ct = default) =>
        _db.BenefitEnrollments.AddAsync(enrollment, ct).AsTask();

    public void UpdateEnrollment(BenefitEnrollment enrollment) => _db.BenefitEnrollments.Update(enrollment);
}
