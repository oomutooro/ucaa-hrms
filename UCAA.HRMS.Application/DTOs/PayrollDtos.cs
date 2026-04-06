namespace UCAA.HRMS.Application.DTOs;

public sealed record PayrollRecordDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    decimal BasicSalary,
    decimal Allowances,
    decimal Deductions,
    decimal NetPay,
    DateOnly PayPeriod,
    string? Notes);

public sealed record CreatePayrollRecordRequest(
    Guid EmployeeId,
    decimal BasicSalary,
    decimal Allowances,
    decimal Deductions,
    DateOnly PayPeriod,
    string? Notes);
