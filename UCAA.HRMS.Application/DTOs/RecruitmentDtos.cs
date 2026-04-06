using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

// ── Job Requisition ───────────────────────────────────────────────────────────
public sealed record JobRequisitionDto(
    Guid Id,
    string RequisitionNumber,
    Guid PositionId,
    string PositionTitle,
    Guid DepartmentId,
    string DepartmentName,
    int VacanciesRequested,
    string Justification,
    DateOnly ClosingDate,
    RequisitionStatus Status,
    string StatusLabel,
    int ApplicationCount);

public sealed record CreateJobRequisitionRequest(
    Guid PositionId,
    Guid DepartmentId,
    int VacanciesRequested,
    string Justification,
    DateOnly ClosingDate);

public sealed record UpdateRequisitionStatusRequest(RequisitionStatus Status);

// ── Job Application ───────────────────────────────────────────────────────────
public sealed record JobApplicationDto(
    Guid Id,
    Guid RequisitionId,
    string RequisitionNumber,
    string PositionTitle,
    string ApplicantName,
    string ApplicantEmail,
    string ApplicantPhone,
    bool IsInternal,
    Guid? EmployeeId,
    ApplicationStatus Status,
    string StatusLabel,
    string? ReviewNotes,
    DateOnly? InterviewDate,
    DateTime AppliedAtUtc);

public sealed record CreateJobApplicationRequest(
    Guid RequisitionId,
    string ApplicantName,
    string ApplicantEmail,
    string ApplicantPhone,
    bool IsInternal,
    Guid? EmployeeId);

public sealed record UpdateApplicationStatusRequest(
    ApplicationStatus Status,
    string? ReviewNotes,
    DateOnly? InterviewDate);
