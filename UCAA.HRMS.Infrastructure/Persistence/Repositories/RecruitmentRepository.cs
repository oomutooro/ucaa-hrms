using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class RecruitmentRepository : IRecruitmentRepository
{
    private readonly ApplicationDbContext _db;

    public RecruitmentRepository(ApplicationDbContext db) => _db = db;

    // ── Requisitions ──────────────────────────────────────────────────────────

    public Task<List<JobRequisition>> ListRequisitionsAsync(CancellationToken ct = default) =>
        _db.JobRequisitions
            .Include(r => r.Position)
            .Include(r => r.Department)
            .Include(r => r.Applications)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(ct);

    public Task<JobRequisition?> GetRequisitionByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.JobRequisitions
            .Include(r => r.Position)
            .Include(r => r.Department)
            .Include(r => r.Applications)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task AddRequisitionAsync(JobRequisition requisition, CancellationToken ct = default) =>
        _db.JobRequisitions.AddAsync(requisition, ct).AsTask();

    public void UpdateRequisition(JobRequisition requisition) => _db.JobRequisitions.Update(requisition);

    public void RemoveRequisition(JobRequisition requisition) => _db.JobRequisitions.Remove(requisition);

    public Task<bool> RequisitionHasApplicationsAsync(Guid requisitionId, CancellationToken ct = default) =>
        _db.JobApplications.AnyAsync(a => a.RequisitionId == requisitionId, ct);

    public Task<int> CountRequisitionsInYearAsync(int year, CancellationToken ct = default) =>
        _db.JobRequisitions.CountAsync(r => r.CreatedAtUtc.Year == year, ct);

    // ── Applications ──────────────────────────────────────────────────────────

    public Task<List<JobApplication>> ListApplicationsAsync(Guid? requisitionId = null, CancellationToken ct = default)
    {
        var query = _db.JobApplications
            .Include(a => a.Requisition)
                .ThenInclude(r => r!.Position)
            .AsQueryable();

        if (requisitionId.HasValue)
            query = query.Where(a => a.RequisitionId == requisitionId.Value);

        return query.OrderByDescending(a => a.CreatedAtUtc).ToListAsync(ct);
    }

    public Task<JobApplication?> GetApplicationByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.JobApplications
            .Include(a => a.Requisition)
                .ThenInclude(r => r!.Position)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task AddApplicationAsync(JobApplication application, CancellationToken ct = default) =>
        _db.JobApplications.AddAsync(application, ct).AsTask();

    public void UpdateApplication(JobApplication application) => _db.JobApplications.Update(application);

    public void RemoveApplication(JobApplication application) => _db.JobApplications.Remove(application);
}
