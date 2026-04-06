using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record BenefitPlanDto(
    Guid Id,
    string Name,
    BenefitPlanType PlanType,
    string PlanTypeLabel,
    string Description,
    bool IsTaxable,
    bool IsActive,
    decimal DefaultEmployerContribution,
    decimal DefaultEmployeeContribution,
    int ActiveEnrollments);

public sealed record CreateBenefitPlanRequest(
    string Name,
    BenefitPlanType PlanType,
    string Description,
    bool IsTaxable,
    decimal DefaultEmployerContribution,
    decimal DefaultEmployeeContribution);

public sealed record UpdateBenefitPlanRequest(
    string Name,
    BenefitPlanType PlanType,
    string Description,
    bool IsTaxable,
    bool IsActive,
    decimal DefaultEmployerContribution,
    decimal DefaultEmployeeContribution);

public sealed record BenefitEnrollmentDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeNumber,
    int EmployeeJobLevel,
    string EmployeeJobTitle,
    string SalaryGradeCode,
    string SalaryGradeTitle,
    Guid BenefitPlanId,
    string BenefitPlanName,
    BenefitPlanType BenefitPlanType,
    string BenefitPlanTypeLabel,
    DateOnly StartDate,
    DateOnly? EndDate,
    BenefitEnrollmentStatus Status,
    string StatusLabel,
    decimal EmployerContribution,
    decimal EmployeeContribution,
    decimal TotalContribution,
    string Notes);

public sealed record CreateBenefitEnrollmentRequest(
    Guid EmployeeId,
    Guid BenefitPlanId,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal? EmployerContribution,
    decimal? EmployeeContribution,
    string? Notes);

public sealed record UpdateBenefitEnrollmentStatusRequest(
    BenefitEnrollmentStatus Status,
    DateOnly? EndDate,
    string? Notes);

public sealed record BenefitSummaryDto(
    int PlanCount,
    int ActivePlanCount,
    int EnrollmentCount,
    int ActiveEnrollmentCount,
    decimal TotalMonthlyEmployerContribution,
    decimal TotalMonthlyEmployeeContribution,
    decimal TotalMonthlyContribution,
    Dictionary<string, int> EnrollmentsByPlanType,
    Dictionary<string, int> ActiveEnrollmentsBySalaryGrade);
