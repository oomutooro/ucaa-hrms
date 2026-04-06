using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record PayrollRecordDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    decimal BasicSalary,
    decimal TransportAllowance,
    decimal HousingAllowance,
    decimal OtherAllowance,
    decimal Allowances,
    decimal PayeTax,
    decimal PensionDeduction,
    decimal LoanDeduction,
    decimal OtherDeduction,
    decimal Deductions,
    decimal GrossPay,
    decimal NetPay,
    DateOnly PayPeriod,
    PayrollStatus Status,
    string StatusLabel,
    DateTime? PaidAtUtc,
    string? Notes);

public sealed record CreatePayrollRecordRequest(
    Guid EmployeeId,
    decimal BasicSalary,
    decimal TransportAllowance,
    decimal HousingAllowance,
    decimal OtherAllowance,
    decimal PayeTax,
    decimal PensionDeduction,
    decimal LoanDeduction,
    decimal OtherDeduction,
    DateOnly PayPeriod,
    string? Notes);

public sealed record UpdatePayrollStatusRequest(PayrollStatus Status, string? Notes);

public sealed record PayrollSummaryDto(
    int RecordCount,
    int DraftCount,
    int ApprovedCount,
    int PaidCount,
    decimal TotalGrossPay,
    decimal TotalDeductions,
    decimal TotalNetPay,
    decimal TotalPaye,
    decimal TotalPension,
    DateOnly? LatestPayPeriod);
