using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record EmployeeDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string EmployeeId,
    Guid DepartmentId,
    string DepartmentName,
    string JobTitle,
    EmploymentType EmploymentType,
    decimal AnnualLeaveBalanceDays);

public sealed record CreateEmployeeRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string EmployeeId,
    Guid DepartmentId,
    string JobTitle,
    EmploymentType EmploymentType,
    decimal? InitialLeaveBalanceDays);

public sealed record UpdateEmployeeRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    Guid DepartmentId,
    string JobTitle,
    EmploymentType EmploymentType);
