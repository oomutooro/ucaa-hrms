using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public sealed class BenefitService : IBenefitService
{
    private readonly IBenefitRepository _repo;
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _unitOfWork;

    public BenefitService(IBenefitRepository repo, IEmployeeRepository employees, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _employees = employees;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BenefitPlanDto>> ListPlansAsync(CancellationToken ct = default)
    {
        var plans = await _repo.ListPlansAsync(ct);
        return plans.Select(MapPlan).ToList();
    }

    public async Task<BenefitPlanDto> CreatePlanAsync(CreateBenefitPlanRequest request, CancellationToken ct = default)
    {
        var normalizedName = request.Name.Trim();

        if (await _repo.PlanNameExistsAsync(normalizedName, ct: ct))
            throw new AppException("Benefit plan name already exists.");

        var plan = new BenefitPlan
        {
            Name = normalizedName,
            PlanType = request.PlanType,
            Description = request.Description.Trim(),
            IsTaxable = request.IsTaxable,
            DefaultEmployerContribution = request.DefaultEmployerContribution,
            DefaultEmployeeContribution = request.DefaultEmployeeContribution,
            IsActive = true
        };

        await _repo.AddPlanAsync(plan, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _repo.GetPlanByIdAsync(plan.Id, ct)
            ?? throw new AppException("Benefit plan could not be loaded after creation.", 500);

        return MapPlan(saved);
    }

    public async Task<BenefitPlanDto> UpdatePlanAsync(Guid id, UpdateBenefitPlanRequest request, CancellationToken ct = default)
    {
        var plan = await _repo.GetPlanByIdAsync(id, ct)
            ?? throw new AppException("Benefit plan not found.", 404);

        var normalizedName = request.Name.Trim();
        if (await _repo.PlanNameExistsAsync(normalizedName, id, ct))
            throw new AppException("Benefit plan name already exists.");

        plan.Name = normalizedName;
        plan.PlanType = request.PlanType;
        plan.Description = request.Description.Trim();
        plan.IsTaxable = request.IsTaxable;
        plan.IsActive = request.IsActive;
        plan.DefaultEmployerContribution = request.DefaultEmployerContribution;
        plan.DefaultEmployeeContribution = request.DefaultEmployeeContribution;
        plan.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdatePlan(plan);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _repo.GetPlanByIdAsync(id, ct)
            ?? throw new AppException("Benefit plan not found after update.", 500);

        return MapPlan(updated);
    }

    public async Task<List<BenefitEnrollmentDto>> ListEnrollmentsAsync(CancellationToken ct = default)
    {
        var enrollments = await _repo.ListEnrollmentsAsync(ct);
        return enrollments.Select(MapEnrollment).ToList();
    }

    public async Task<BenefitEnrollmentDto> CreateEnrollmentAsync(CreateBenefitEnrollmentRequest request, CancellationToken ct = default)
    {
        var employee = await _employees.GetByIdAsync(request.EmployeeId, ct)
            ?? throw new AppException("Employee not found.", 404);

        var plan = await _repo.GetPlanByIdAsync(request.BenefitPlanId, ct)
            ?? throw new AppException("Benefit plan not found.", 404);

        if (!plan.IsActive)
            throw new AppException("Cannot enroll into an inactive benefit plan.");

        if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
            throw new AppException("Enrollment end date must be on or after start date.");

        var enrollment = new BenefitEnrollment
        {
            EmployeeId = request.EmployeeId,
            BenefitPlanId = request.BenefitPlanId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.EndDate.HasValue ? BenefitEnrollmentStatus.Terminated : BenefitEnrollmentStatus.Active,
            EmployerContribution = request.EmployerContribution ?? plan.DefaultEmployerContribution,
            EmployeeContribution = request.EmployeeContribution ?? plan.DefaultEmployeeContribution,
            Notes = request.Notes?.Trim() ?? string.Empty
        };

        await _repo.AddEnrollmentAsync(enrollment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _repo.GetEnrollmentByIdAsync(enrollment.Id, ct)
            ?? throw new AppException("Enrollment could not be loaded after creation.", 500);

        return MapEnrollment(saved);
    }

    public async Task<BenefitEnrollmentDto> UpdateEnrollmentStatusAsync(Guid id, UpdateBenefitEnrollmentStatusRequest request, CancellationToken ct = default)
    {
        var enrollment = await _repo.GetEnrollmentByIdAsync(id, ct)
            ?? throw new AppException("Benefit enrollment not found.", 404);

        if (request.Status == BenefitEnrollmentStatus.Terminated && !request.EndDate.HasValue && enrollment.EndDate is null)
            throw new AppException("Termination requires an end date.");

        enrollment.Status = request.Status;
        enrollment.EndDate = request.EndDate ?? enrollment.EndDate;
        enrollment.Notes = request.Notes?.Trim() ?? enrollment.Notes;
        enrollment.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdateEnrollment(enrollment);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _repo.GetEnrollmentByIdAsync(id, ct)
            ?? throw new AppException("Benefit enrollment not found after update.", 500);

        return MapEnrollment(updated);
    }

    public async Task<BenefitSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var plans = await _repo.ListPlansAsync(ct);
        var enrollments = await _repo.ListEnrollmentsAsync(ct);

        var activeEnrollments = enrollments.Where(e => e.Status == BenefitEnrollmentStatus.Active).ToList();

        var planTypeStats = enrollments
            .GroupBy(e => e.BenefitPlan?.PlanType ?? BenefitPlanType.Other)
            .ToDictionary(g => PlanTypeLabel(g.Key), g => g.Count());

        var salaryGradeStats = activeEnrollments
            .GroupBy(e => e.Employee?.Position?.JobDescription?.JobGrade?.GradeCode ?? $"L{e.Employee?.JobLevel ?? 0}")
            .ToDictionary(g => g.Key, g => g.Count());

        var totalEmployer = activeEnrollments.Sum(e => e.EmployerContribution);
        var totalEmployee = activeEnrollments.Sum(e => e.EmployeeContribution);

        return new BenefitSummaryDto(
            plans.Count,
            plans.Count(p => p.IsActive),
            enrollments.Count,
            activeEnrollments.Count,
            totalEmployer,
            totalEmployee,
            totalEmployer + totalEmployee,
            planTypeStats,
            salaryGradeStats);
    }

    private static BenefitPlanDto MapPlan(BenefitPlan plan) =>
        new(
            plan.Id,
            plan.Name,
            plan.PlanType,
            PlanTypeLabel(plan.PlanType),
            plan.Description,
            plan.IsTaxable,
            plan.IsActive,
            plan.DefaultEmployerContribution,
            plan.DefaultEmployeeContribution,
            plan.Enrollments.Count(e => e.Status == BenefitEnrollmentStatus.Active));

    private static BenefitEnrollmentDto MapEnrollment(BenefitEnrollment enrollment)
    {
        var salaryGradeCode = enrollment.Employee?.Position?.JobDescription?.JobGrade?.GradeCode ?? $"L{enrollment.Employee?.JobLevel ?? 0}";
        var salaryGradeTitle = enrollment.Employee?.Position?.JobDescription?.JobGrade?.GradeTitle ?? $"Level {enrollment.Employee?.JobLevel ?? 0}";

        return new BenefitEnrollmentDto(
            enrollment.Id,
            enrollment.EmployeeId,
            enrollment.Employee?.FullName ?? string.Empty,
            enrollment.Employee?.EmployeeId ?? string.Empty,
            enrollment.Employee?.JobLevel ?? 0,
            enrollment.Employee?.JobTitle ?? string.Empty,
            salaryGradeCode,
            salaryGradeTitle,
            enrollment.BenefitPlanId,
            enrollment.BenefitPlan?.Name ?? string.Empty,
            enrollment.BenefitPlan?.PlanType ?? BenefitPlanType.Other,
            PlanTypeLabel(enrollment.BenefitPlan?.PlanType ?? BenefitPlanType.Other),
            enrollment.StartDate,
            enrollment.EndDate,
            enrollment.Status,
            EnrollmentStatusLabel(enrollment.Status),
            enrollment.EmployerContribution,
            enrollment.EmployeeContribution,
            enrollment.EmployerContribution + enrollment.EmployeeContribution,
            enrollment.Notes);
    }

    private static string PlanTypeLabel(BenefitPlanType type) => type switch
    {
        BenefitPlanType.Medical => "Medical",
        BenefitPlanType.Pension => "Pension",
        BenefitPlanType.LifeInsurance => "Life Insurance",
        BenefitPlanType.Transport => "Transport",
        BenefitPlanType.Meal => "Meal",
        BenefitPlanType.Housing => "Housing",
        BenefitPlanType.Education => "Education",
        BenefitPlanType.Other => "Other",
        _ => type.ToString()
    };

    private static string EnrollmentStatusLabel(BenefitEnrollmentStatus status) => status switch
    {
        BenefitEnrollmentStatus.Active => "Active",
        BenefitEnrollmentStatus.Suspended => "Suspended",
        BenefitEnrollmentStatus.Terminated => "Terminated",
        _ => status.ToString()
    };
}
