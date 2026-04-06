using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public sealed class PayrollService : IPayrollService
{
    private readonly IPayrollRepository _payroll;
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _unitOfWork;

    public PayrollService(IPayrollRepository payroll, IEmployeeRepository employees, IUnitOfWork unitOfWork)
    {
        _payroll = payroll;
        _employees = employees;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<PayrollRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _payroll.ListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<PayrollSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var items = await _payroll.ListAsync(cancellationToken);
        return new PayrollSummaryDto(
            items.Count,
            items.Count(x => x.Status == PayrollStatus.Draft),
            items.Count(x => x.Status == PayrollStatus.Approved),
            items.Count(x => x.Status == PayrollStatus.Paid),
            items.Sum(x => x.GrossPay),
            items.Sum(x => x.Deductions),
            items.Sum(x => x.NetPay),
            items.Sum(x => x.PayeTax),
            items.Sum(x => x.PensionDeduction),
            items.Count == 0 ? null : items.Max(x => x.PayPeriod));
    }

    public async Task<PayrollRecordDto> CreateAsync(CreatePayrollRecordRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new AppException("Employee not found.", 404);

        if (await _payroll.ExistsForEmployeeAndPeriodAsync(request.EmployeeId, request.PayPeriod, cancellationToken: cancellationToken))
        {
            throw new AppException("A payroll record already exists for this employee and pay period.");
        }

        var allowances = request.TransportAllowance + request.HousingAllowance + request.OtherAllowance;
        var deductions = request.PayeTax + request.PensionDeduction + request.LoanDeduction + request.OtherDeduction;
        var grossPay = request.BasicSalary + allowances;
        var netPay = grossPay - deductions;

        if (deductions > grossPay * 0.5m)
        {
            throw new AppException("Total deductions cannot exceed 50% of gross monthly pay.");
        }

        if (netPay < 0)
        {
            throw new AppException("Net pay cannot be negative.");
        }

        var record = new PayrollRecord
        {
            EmployeeId = request.EmployeeId,
            BasicSalary = request.BasicSalary,
            TransportAllowance = request.TransportAllowance,
            HousingAllowance = request.HousingAllowance,
            OtherAllowance = request.OtherAllowance,
            Allowances = allowances,
            PayeTax = request.PayeTax,
            PensionDeduction = request.PensionDeduction,
            LoanDeduction = request.LoanDeduction,
            OtherDeduction = request.OtherDeduction,
            Deductions = deductions,
            GrossPay = grossPay,
            NetPay = netPay,
            PayPeriod = request.PayPeriod,
            Status = PayrollStatus.Draft,
            Notes = request.Notes
        };

        await _payroll.AddAsync(record, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        record.Employee = employee;
        return Map(record);
    }

    public async Task<PayrollRecordDto> UpdateStatusAsync(Guid id, UpdatePayrollStatusRequest request, CancellationToken cancellationToken = default)
    {
        var record = await _payroll.GetByIdAsync(id, cancellationToken)
            ?? throw new AppException("Payroll record not found.", 404);

        if (record.Status == PayrollStatus.Paid && request.Status != PayrollStatus.Paid)
        {
            throw new AppException("Paid payroll records cannot be moved back to a non-paid state.");
        }

        record.Status = request.Status;
        record.PaidAtUtc = request.Status == PayrollStatus.Paid ? DateTime.UtcNow : null;
        record.Notes = string.IsNullOrWhiteSpace(request.Notes)
            ? record.Notes
            : request.Notes;
        record.UpdatedAtUtc = DateTime.UtcNow;

        _payroll.Update(record);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(record);
    }

    private static PayrollRecordDto Map(PayrollRecord record) =>
        new(
            record.Id,
            record.EmployeeId,
            record.Employee?.FullName ?? string.Empty,
            record.BasicSalary,
            record.TransportAllowance,
            record.HousingAllowance,
            record.OtherAllowance,
            record.Allowances,
            record.PayeTax,
            record.PensionDeduction,
            record.LoanDeduction,
            record.OtherDeduction,
            record.Deductions,
            record.GrossPay,
            record.NetPay,
            record.PayPeriod,
            record.Status,
            GetStatusLabel(record.Status),
            record.PaidAtUtc,
            record.Notes);

    private static string GetStatusLabel(PayrollStatus status) => status switch
    {
        PayrollStatus.Draft => "Draft",
        PayrollStatus.Approved => "Approved",
        PayrollStatus.Paid => "Paid",
        _ => status.ToString()
    };
}
