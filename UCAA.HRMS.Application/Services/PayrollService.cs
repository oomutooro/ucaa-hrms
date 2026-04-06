using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;

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

    public async Task<PayrollRecordDto> CreateAsync(CreatePayrollRecordRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new AppException("Employee not found.", 404);

        var record = new PayrollRecord
        {
            EmployeeId = request.EmployeeId,
            BasicSalary = request.BasicSalary,
            Allowances = request.Allowances,
            Deductions = request.Deductions,
            PayPeriod = request.PayPeriod,
            Notes = request.Notes
        };

        await _payroll.AddAsync(record, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        record.Employee = employee;
        return Map(record);
    }

    private static PayrollRecordDto Map(PayrollRecord record) =>
        new(
            record.Id,
            record.EmployeeId,
            record.Employee?.FullName ?? string.Empty,
            record.BasicSalary,
            record.Allowances,
            record.Deductions,
            record.BasicSalary + record.Allowances - record.Deductions,
            record.PayPeriod,
            record.Notes);
}
