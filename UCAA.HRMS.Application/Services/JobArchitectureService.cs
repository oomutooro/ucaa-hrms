using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Services;

public sealed class JobArchitectureService : IJobArchitectureService
{
    private readonly IJobArchitectureRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public JobArchitectureService(IJobArchitectureRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    // ── Job Grades ────────────────────────────────────────────────────────────

    public async Task<List<JobGradeDto>> ListGradesAsync(CancellationToken ct = default)
    {
        var grades = await _repo.ListGradesAsync(ct);
        return grades.Select(MapGrade).ToList();
    }

    public async Task<JobGradeDto> CreateGradeAsync(CreateJobGradeRequest request, CancellationToken ct = default)
    {
        if (await _repo.GradeCodeExistsAsync(request.GradeCode, ct: ct))
            throw new AppException($"Grade code '{request.GradeCode}' already exists.");

        if (request.MinSalary > request.MaxSalary)
            throw new AppException("Minimum salary cannot exceed maximum salary.");

        var grade = new JobGrade
        {
            GradeCode = request.GradeCode.Trim(),
            GradeTitle = request.GradeTitle.Trim(),
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary
        };

        await _repo.AddGradeAsync(grade, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapGrade(grade);
    }

    public async Task<JobGradeDto> UpdateGradeAsync(Guid id, UpdateJobGradeRequest request, CancellationToken ct = default)
    {
        var grade = await _repo.GetGradeByIdAsync(id, ct)
            ?? throw new AppException("Job grade not found.", 404);

        if (await _repo.GradeCodeExistsAsync(request.GradeCode, excludeId: id, ct: ct))
            throw new AppException($"Grade code '{request.GradeCode}' already exists.");

        if (request.MinSalary > request.MaxSalary)
            throw new AppException("Minimum salary cannot exceed maximum salary.");

        grade.GradeCode = request.GradeCode.Trim();
        grade.GradeTitle = request.GradeTitle.Trim();
        grade.MinSalary = request.MinSalary;
        grade.MaxSalary = request.MaxSalary;
        grade.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdateGrade(grade);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapGrade(grade);
    }

    public async Task DeleteGradeAsync(Guid id, CancellationToken ct = default)
    {
        var grade = await _repo.GetGradeByIdAsync(id, ct)
            ?? throw new AppException("Job grade not found.", 404);

        if (await _repo.GradeHasJobDescriptionsAsync(id, ct))
            throw new AppException("Cannot delete a grade that has job descriptions linked to it.");

        _repo.RemoveGrade(grade);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ── Job Descriptions ──────────────────────────────────────────────────────

    public async Task<List<JobDescriptionDto>> ListJobDescriptionsAsync(CancellationToken ct = default)
    {
        var jds = await _repo.ListJobDescriptionsAsync(ct);
        return jds.Select(MapJd).ToList();
    }

    public async Task<JobDescriptionDto> CreateJobDescriptionAsync(CreateJobDescriptionRequest request, CancellationToken ct = default)
    {
        var grade = await _repo.GetGradeByIdAsync(request.JobGradeId, ct)
            ?? throw new AppException("Job grade not found.", 404);

        var jd = new JobDescription
        {
            Title = request.Title.Trim(),
            PurposeStatement = request.PurposeStatement.Trim(),
            KeyAccountabilities = request.KeyAccountabilities.Trim(),
            Qualifications = request.Qualifications.Trim(),
            JobGradeId = grade.Id
        };

        await _repo.AddJobDescriptionAsync(jd, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        jd.JobGrade = grade;
        return MapJd(jd);
    }

    public async Task<JobDescriptionDto> UpdateJobDescriptionAsync(Guid id, UpdateJobDescriptionRequest request, CancellationToken ct = default)
    {
        var jd = await _repo.GetJobDescriptionByIdAsync(id, ct)
            ?? throw new AppException("Job description not found.", 404);

        var grade = await _repo.GetGradeByIdAsync(request.JobGradeId, ct)
            ?? throw new AppException("Job grade not found.", 404);

        jd.Title = request.Title.Trim();
        jd.PurposeStatement = request.PurposeStatement.Trim();
        jd.KeyAccountabilities = request.KeyAccountabilities.Trim();
        jd.Qualifications = request.Qualifications.Trim();
        jd.JobGradeId = grade.Id;
        jd.JobGrade = grade;
        jd.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdateJobDescription(jd);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapJd(jd);
    }

    public async Task DeleteJobDescriptionAsync(Guid id, CancellationToken ct = default)
    {
        var jd = await _repo.GetJobDescriptionByIdAsync(id, ct)
            ?? throw new AppException("Job description not found.", 404);

        if (await _repo.JobDescriptionHasPositionsAsync(id, ct))
            throw new AppException("Cannot delete a job description that has positions linked to it.");

        _repo.RemoveJobDescription(jd);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ── Positions ─────────────────────────────────────────────────────────────

    public async Task<List<PositionDto>> ListPositionsAsync(CancellationToken ct = default)
    {
        var positions = await _repo.ListPositionsAsync(ct);
        return positions.Select(MapPosition).ToList();
    }

    public async Task<PositionDto> CreatePositionAsync(CreatePositionRequest request, CancellationToken ct = default)
    {
        var jd = await _repo.GetJobDescriptionByIdAsync(request.JobDescriptionId, ct)
            ?? throw new AppException("Job description not found.", 404);

        if (request.ApprovedHeadcount < 1)
            throw new AppException("Approved headcount must be at least 1.");

        var position = new Position
        {
            Title = request.Title.Trim(),
            DepartmentId = request.DepartmentId,
            JobDescriptionId = jd.Id,
            ApprovedHeadcount = request.ApprovedHeadcount
        };

        await _repo.AddPositionAsync(position, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _repo.GetPositionByIdAsync(position.Id, ct)
            ?? throw new AppException("Failed to retrieve saved position.", 500);

        return MapPosition(saved);
    }

    public async Task<PositionDto> UpdatePositionAsync(Guid id, UpdatePositionRequest request, CancellationToken ct = default)
    {
        var position = await _repo.GetPositionByIdAsync(id, ct)
            ?? throw new AppException("Position not found.", 404);

        var jd = await _repo.GetJobDescriptionByIdAsync(request.JobDescriptionId, ct)
            ?? throw new AppException("Job description not found.", 404);

        if (request.ApprovedHeadcount < 1)
            throw new AppException("Approved headcount must be at least 1.");

        position.Title = request.Title.Trim();
        position.DepartmentId = request.DepartmentId;
        position.JobDescriptionId = jd.Id;
        position.ApprovedHeadcount = request.ApprovedHeadcount;
        position.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdatePosition(position);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _repo.GetPositionByIdAsync(id, ct)!;
        return MapPosition(updated!);
    }

    public async Task DeletePositionAsync(Guid id, CancellationToken ct = default)
    {
        var position = await _repo.GetPositionByIdAsync(id, ct)
            ?? throw new AppException("Position not found.", 404);

        _repo.RemovePosition(position);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ── Mapping helpers ───────────────────────────────────────────────────────

    private static JobGradeDto MapGrade(JobGrade g) =>
        new(g.Id, g.GradeCode, g.GradeTitle, g.MinSalary, g.MaxSalary);

    private static JobDescriptionDto MapJd(JobDescription j) =>
        new(j.Id, j.Title, j.PurposeStatement, j.KeyAccountabilities, j.Qualifications,
            j.JobGradeId, j.JobGrade?.GradeCode ?? string.Empty, j.JobGrade?.GradeTitle ?? string.Empty);

    private static PositionDto MapPosition(Position p) =>
        new(p.Id, p.Title,
            p.DepartmentId, p.Department?.Name ?? string.Empty,
            p.JobDescriptionId, p.JobDescription?.Title ?? string.Empty,
            p.JobDescription?.JobGrade?.GradeCode ?? string.Empty,
            p.ApprovedHeadcount);
}
