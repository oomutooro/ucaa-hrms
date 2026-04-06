using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public sealed class RecruitmentService : IRecruitmentService
{
    private readonly IRecruitmentRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public RecruitmentService(IRecruitmentRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    // ── Requisitions ──────────────────────────────────────────────────────────

    public async Task<List<JobRequisitionDto>> ListRequisitionsAsync(CancellationToken ct = default)
    {
        var items = await _repo.ListRequisitionsAsync(ct);
        return items.Select(MapRequisition).ToList();
    }

    public async Task<JobRequisitionDto> CreateRequisitionAsync(CreateJobRequisitionRequest request, CancellationToken ct = default)
    {
        if (request.VacanciesRequested < 1)
            throw new AppException("Vacancies requested must be at least 1.");

        if (request.ClosingDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new AppException("Closing date cannot be in the past.");

        var number = await GenerateRequisitionNumberAsync(ct);

        var req = new JobRequisition
        {
            RequisitionNumber = number,
            PositionId = request.PositionId,
            DepartmentId = request.DepartmentId,
            VacanciesRequested = request.VacanciesRequested,
            Justification = request.Justification.Trim(),
            ClosingDate = request.ClosingDate,
            Status = RequisitionStatus.Draft
        };

        await _repo.AddRequisitionAsync(req, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _repo.GetRequisitionByIdAsync(req.Id, ct)!;
        return MapRequisition(saved!);
    }

    public async Task<JobRequisitionDto> UpdateRequisitionStatusAsync(Guid id, UpdateRequisitionStatusRequest request, CancellationToken ct = default)
    {
        var req = await _repo.GetRequisitionByIdAsync(id, ct)
            ?? throw new AppException("Requisition not found.", 404);

        // Guard: only Draft can be opened; only Open can be closed/cancelled
        if (request.Status == RequisitionStatus.Open && req.Status != RequisitionStatus.Draft)
            throw new AppException("Only a Draft requisition can be opened.");

        if (request.Status == RequisitionStatus.Closed && req.Status != RequisitionStatus.Open)
            throw new AppException("Only an Open requisition can be closed.");

        req.Status = request.Status;
        req.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdateRequisition(req);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _repo.GetRequisitionByIdAsync(id, ct)!;
        return MapRequisition(updated!);
    }

    public async Task DeleteRequisitionAsync(Guid id, CancellationToken ct = default)
    {
        var req = await _repo.GetRequisitionByIdAsync(id, ct)
            ?? throw new AppException("Requisition not found.", 404);

        if (req.Status == RequisitionStatus.Open)
            throw new AppException("Close or cancel the requisition before deleting it.");

        if (await _repo.RequisitionHasApplicationsAsync(id, ct))
            throw new AppException("Cannot delete a requisition that has applications.");

        _repo.RemoveRequisition(req);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ── Applications ──────────────────────────────────────────────────────────

    public async Task<List<JobApplicationDto>> ListApplicationsAsync(Guid? requisitionId = null, CancellationToken ct = default)
    {
        var items = await _repo.ListApplicationsAsync(requisitionId, ct);
        return items.Select(MapApplication).ToList();
    }

    public async Task<JobApplicationDto> CreateApplicationAsync(CreateJobApplicationRequest request, CancellationToken ct = default)
    {
        var req = await _repo.GetRequisitionByIdAsync(request.RequisitionId, ct)
            ?? throw new AppException("Requisition not found.", 404);

        if (req.Status != RequisitionStatus.Open)
            throw new AppException("Applications can only be submitted against an Open requisition.");

        if (request.IsInternal && !request.EmployeeId.HasValue)
            throw new AppException("Internal applications must reference an existing employee.");

        var app = new JobApplication
        {
            RequisitionId = request.RequisitionId,
            ApplicantName = request.ApplicantName.Trim(),
            ApplicantEmail = request.ApplicantEmail.Trim(),
            ApplicantPhone = request.ApplicantPhone.Trim(),
            IsInternal = request.IsInternal,
            EmployeeId = request.IsInternal ? request.EmployeeId : null,
            Status = ApplicationStatus.Received
        };

        await _repo.AddApplicationAsync(app, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _repo.GetApplicationByIdAsync(app.Id, ct)!;
        return MapApplication(saved!);
    }

    public async Task<JobApplicationDto> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request, CancellationToken ct = default)
    {
        var app = await _repo.GetApplicationByIdAsync(id, ct)
            ?? throw new AppException("Application not found.", 404);

        if (request.Status == ApplicationStatus.InterviewScheduled && !request.InterviewDate.HasValue)
            throw new AppException("An interview date is required when scheduling an interview.");

        app.Status = request.Status;
        app.ReviewNotes = request.ReviewNotes?.Trim();
        app.InterviewDate = request.InterviewDate;
        app.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdateApplication(app);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _repo.GetApplicationByIdAsync(id, ct)!;
        return MapApplication(updated!);
    }

    public async Task DeleteApplicationAsync(Guid id, CancellationToken ct = default)
    {
        var app = await _repo.GetApplicationByIdAsync(id, ct)
            ?? throw new AppException("Application not found.", 404);

        _repo.RemoveApplication(app);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<string> GenerateRequisitionNumberAsync(CancellationToken ct)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _repo.CountRequisitionsInYearAsync(year, ct);
        return $"REQ-{year}-{(count + 1):D4}";
    }

    private static string RequisitionStatusLabel(RequisitionStatus s) => s switch
    {
        RequisitionStatus.Draft => "Draft",
        RequisitionStatus.Open => "Open",
        RequisitionStatus.Closed => "Closed",
        RequisitionStatus.Cancelled => "Cancelled",
        _ => s.ToString()
    };

    private static string ApplicationStatusLabel(ApplicationStatus s) => s switch
    {
        ApplicationStatus.Received => "Received",
        ApplicationStatus.Shortlisted => "Shortlisted",
        ApplicationStatus.InterviewScheduled => "Interview Scheduled",
        ApplicationStatus.Offered => "Offered",
        ApplicationStatus.Hired => "Hired",
        ApplicationStatus.Rejected => "Rejected",
        _ => s.ToString()
    };

    private static JobRequisitionDto MapRequisition(JobRequisition r) =>
        new(r.Id, r.RequisitionNumber,
            r.PositionId, r.Position?.Title ?? string.Empty,
            r.DepartmentId, r.Department?.Name ?? string.Empty,
            r.VacanciesRequested, r.Justification, r.ClosingDate,
            r.Status, RequisitionStatusLabel(r.Status),
            r.Applications.Count);

    private static JobApplicationDto MapApplication(JobApplication a) =>
        new(a.Id, a.RequisitionId,
            a.Requisition?.RequisitionNumber ?? string.Empty,
            a.Requisition?.Position?.Title ?? string.Empty,
            a.ApplicantName, a.ApplicantEmail, a.ApplicantPhone,
            a.IsInternal, a.EmployeeId,
            a.Status, ApplicationStatusLabel(a.Status),
            a.ReviewNotes, a.InterviewDate, a.CreatedAtUtc);
}
