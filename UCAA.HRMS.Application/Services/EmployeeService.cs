using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;
    private readonly IDepartmentRepository _departments;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IEmployeeRepository employees, IDepartmentRepository departments, IUnitOfWork unitOfWork)
    {
        _employees = employees;
        _departments = departments;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EmployeeDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var data = await _employees.ListAsync(cancellationToken);
        return data.Select(Map).ToList();
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        if (await _employees.EmailExistsAsync(request.Email, cancellationToken: cancellationToken))
        {
            throw new AppException("Employee email already exists.");
        }

        if (await _employees.EmployeeIdentifierExistsAsync(request.EmployeeId, cancellationToken: cancellationToken))
        {
            throw new AppException("Employee ID already exists.");
        }

        var department = await _departments.GetByIdAsync(request.DepartmentId, cancellationToken)
            ?? throw new AppException("Department not found.", 404);

        var employee = new Employee
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            EmployeeId = request.EmployeeId,
            DepartmentId = request.DepartmentId,
            JobTitle = request.JobTitle,
            EmploymentType = request.EmploymentType,
            AnnualLeaveBalanceDays = request.InitialLeaveBalanceDays ?? 30
        };

        await _employees.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        employee.Department = department;
        return Map(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken)
            ?? throw new AppException("Employee not found.", 404);

        if (await _employees.EmailExistsAsync(request.Email, id, cancellationToken))
        {
            throw new AppException("Employee email already exists.");
        }

        var department = await _departments.GetByIdAsync(request.DepartmentId, cancellationToken)
            ?? throw new AppException("Department not found.", 404);

        employee.FullName = request.FullName;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.DepartmentId = request.DepartmentId;
        employee.JobTitle = request.JobTitle;
        employee.EmploymentType = request.EmploymentType;
        employee.UpdatedAtUtc = DateTime.UtcNow;

        _employees.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        employee.Department = department;
        return Map(employee);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken)
            ?? throw new AppException("Employee not found.", 404);

        _employees.Remove(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static EmployeeDto Map(Employee employee) =>
        new(
            employee.Id,
            employee.FullName,
            employee.Email,
            employee.PhoneNumber,
            employee.EmployeeId,
            employee.DepartmentId,
            employee.Department?.Name ?? string.Empty,
            employee.JobTitle,
            employee.EmploymentType,
            employee.AnnualLeaveBalanceDays);
}
