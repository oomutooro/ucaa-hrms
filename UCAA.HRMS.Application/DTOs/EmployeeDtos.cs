using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record EmployeeDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string EmployeeId,
    DateOnly DateOfBirth,
    DateOnly FirstEmploymentDate,
    int JobLevel,
    int AnnualLeaveEntitlementDays,
    int Age,
    int YearsOfService,
    DateOnly MandatoryRetirementDate,
    bool IsEligibleForVoluntaryRetirement,
    bool IsAtOrAboveMandatoryRetirementAge,
    bool IsEligibleForGoldenHandshake,
    bool IsEligibleForLongServiceAward,
    int NoticePeriodMonths,
    decimal ServiceGratuityMonthsPerCompletedYear,
    decimal ServiceGratuityTotalMonths,
    decimal RedundancySeveranceMonths,
    Guid DepartmentId,
    string DepartmentName,
    Guid? PositionId,
    string PositionTitle,
    string SalaryGradeCode,
    string SalaryGradeTitle,
    string JobTitle,
    EmploymentType EmploymentType,
    decimal AnnualLeaveBalanceDays);

public sealed record CreateEmployeeRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string EmployeeId,
    DateOnly DateOfBirth,
    DateOnly FirstEmploymentDate,
    int JobLevel,
    Guid DepartmentId,
    Guid? PositionId,
    string JobTitle,
    EmploymentType EmploymentType,
    decimal? InitialLeaveBalanceDays);

public sealed record UpdateEmployeeRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth,
    DateOnly FirstEmploymentDate,
    int JobLevel,
    Guid DepartmentId,
    Guid? PositionId,
    string JobTitle,
    EmploymentType EmploymentType);
