namespace UCAA.HRMS.Application.DTOs;

// ── Job Grade ────────────────────────────────────────────────────────────────
public sealed record JobGradeDto(
    Guid Id,
    string GradeCode,
    string GradeTitle,
    decimal MinSalary,
    decimal MaxSalary);

public sealed record CreateJobGradeRequest(
    string GradeCode,
    string GradeTitle,
    decimal MinSalary,
    decimal MaxSalary);

public sealed record UpdateJobGradeRequest(
    string GradeCode,
    string GradeTitle,
    decimal MinSalary,
    decimal MaxSalary);

// ── Job Description ───────────────────────────────────────────────────────────
public sealed record JobDescriptionDto(
    Guid Id,
    string Title,
    string PurposeStatement,
    string KeyAccountabilities,
    string Qualifications,
    Guid JobGradeId,
    string JobGradeCode,
    string JobGradeTitle);

public sealed record CreateJobDescriptionRequest(
    string Title,
    string PurposeStatement,
    string KeyAccountabilities,
    string Qualifications,
    Guid JobGradeId);

public sealed record UpdateJobDescriptionRequest(
    string Title,
    string PurposeStatement,
    string KeyAccountabilities,
    string Qualifications,
    Guid JobGradeId);

// ── Position ──────────────────────────────────────────────────────────────────
public sealed record PositionDto(
    Guid Id,
    string Title,
    Guid DepartmentId,
    string DepartmentName,
    Guid JobDescriptionId,
    string JobDescriptionTitle,
    string JobGradeCode,
    int ApprovedHeadcount);

public sealed record CreatePositionRequest(
    string Title,
    Guid DepartmentId,
    Guid JobDescriptionId,
    int ApprovedHeadcount);

public sealed record UpdatePositionRequest(
    string Title,
    Guid DepartmentId,
    Guid JobDescriptionId,
    int ApprovedHeadcount);
