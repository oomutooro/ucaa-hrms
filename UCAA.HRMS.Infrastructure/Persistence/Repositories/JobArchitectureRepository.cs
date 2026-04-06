using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class JobArchitectureRepository : IJobArchitectureRepository
{
    private readonly ApplicationDbContext _db;

    public JobArchitectureRepository(ApplicationDbContext db) => _db = db;

    // ── Job Grades ────────────────────────────────────────────────────────────

    public Task<List<JobGrade>> ListGradesAsync(CancellationToken ct = default) =>
        _db.JobGrades.OrderBy(g => g.GradeCode).ToListAsync(ct);

    public Task<JobGrade?> GetGradeByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.JobGrades.FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<bool> GradeCodeExistsAsync(string gradeCode, Guid? excludeId = null, CancellationToken ct = default) =>
        _db.JobGrades.AnyAsync(g => g.GradeCode.ToLower() == gradeCode.ToLower()
            && (!excludeId.HasValue || g.Id != excludeId.Value), ct);

    public Task AddGradeAsync(JobGrade grade, CancellationToken ct = default) =>
        _db.JobGrades.AddAsync(grade, ct).AsTask();

    public void UpdateGrade(JobGrade grade) => _db.JobGrades.Update(grade);

    public void RemoveGrade(JobGrade grade) => _db.JobGrades.Remove(grade);

    public Task<bool> GradeHasJobDescriptionsAsync(Guid gradeId, CancellationToken ct = default) =>
        _db.JobDescriptions.AnyAsync(j => j.JobGradeId == gradeId, ct);

    // ── Job Descriptions ──────────────────────────────────────────────────────

    public Task<List<JobDescription>> ListJobDescriptionsAsync(CancellationToken ct = default) =>
        _db.JobDescriptions
            .Include(j => j.JobGrade)
            .OrderBy(j => j.Title)
            .ToListAsync(ct);

    public Task<JobDescription?> GetJobDescriptionByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.JobDescriptions
            .Include(j => j.JobGrade)
            .FirstOrDefaultAsync(j => j.Id == id, ct);

    public Task AddJobDescriptionAsync(JobDescription jd, CancellationToken ct = default) =>
        _db.JobDescriptions.AddAsync(jd, ct).AsTask();

    public void UpdateJobDescription(JobDescription jd) => _db.JobDescriptions.Update(jd);

    public void RemoveJobDescription(JobDescription jd) => _db.JobDescriptions.Remove(jd);

    public Task<bool> JobDescriptionHasPositionsAsync(Guid jobDescriptionId, CancellationToken ct = default) =>
        _db.Positions.AnyAsync(p => p.JobDescriptionId == jobDescriptionId, ct);

    // ── Positions ─────────────────────────────────────────────────────────────

    public Task<List<Position>> ListPositionsAsync(CancellationToken ct = default) =>
        _db.Positions
            .Include(p => p.Department)
            .Include(p => p.JobDescription)
                .ThenInclude(j => j!.JobGrade)
            .OrderBy(p => p.Title)
            .ToListAsync(ct);

    public Task<Position?> GetPositionByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Positions
            .Include(p => p.Department)
            .Include(p => p.JobDescription)
                .ThenInclude(j => j!.JobGrade)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task AddPositionAsync(Position position, CancellationToken ct = default) =>
        _db.Positions.AddAsync(position, ct).AsTask();

    public void UpdatePosition(Position position) => _db.Positions.Update(position);

    public void RemovePosition(Position position) => _db.Positions.Remove(position);
}
