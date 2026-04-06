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

        if (!department.ParentDepartmentId.HasValue)
        {
            throw new AppException("Employees must be assigned to a department or section under a directorate.");
        }

        var employee = new Employee
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            EmployeeId = request.EmployeeId,
            DateOfBirth = request.DateOfBirth,
            FirstEmploymentDate = request.FirstEmploymentDate,
            JobLevel = request.JobLevel,
            DepartmentId = request.DepartmentId,
            JobTitle = request.JobTitle,
            EmploymentType = request.EmploymentType,
            AnnualLeaveBalanceDays = request.InitialLeaveBalanceDays ?? GetAnnualLeaveEntitlementDays(request.JobLevel)
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

        if (!department.ParentDepartmentId.HasValue)
        {
            throw new AppException("Employees must be assigned to a department or section under a directorate.");
        }

        employee.FullName = request.FullName;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.DateOfBirth = request.DateOfBirth;
        employee.FirstEmploymentDate = request.FirstEmploymentDate;
        employee.JobLevel = request.JobLevel;
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

    private static EmployeeDto Map(Employee employee)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - employee.DateOfBirth.Year;
        if (employee.DateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        var yearsOfService = today.Year - employee.FirstEmploymentDate.Year;
        if (employee.FirstEmploymentDate > today.AddYears(-yearsOfService))
        {
            yearsOfService--;
        }

        var clampedAge = Math.Max(age, 0);
        var clampedYearsOfService = Math.Max(yearsOfService, 0);
        var mandatoryRetirementDate = employee.DateOfBirth.AddYears(60);
        var annualLeaveEntitlementDays = GetAnnualLeaveEntitlementDays(employee.JobLevel);
        var isEligibleForVoluntaryRetirement = clampedAge >= 55 && clampedYearsOfService >= 15;
        var isAtOrAboveMandatoryRetirementAge = clampedAge >= 60;
        var isEligibleForGoldenHandshake = isAtOrAboveMandatoryRetirementAge;
        var isEligibleForLongServiceAward = clampedYearsOfService > 0 && clampedYearsOfService % 10 == 0;
        var noticePeriodMonths = GetNoticePeriodMonths(clampedYearsOfService);
        var serviceGratuityMonthsPerCompletedYear = GetServiceGratuityMonthsPerCompletedYear(clampedYearsOfService);
        var serviceGratuityBaseMonths = serviceGratuityMonthsPerCompletedYear * clampedYearsOfService;
        var serviceGratuityBonusMonths = (clampedYearsOfService / 10) * 2;
        var serviceGratuityTotalMonths = serviceGratuityBaseMonths + serviceGratuityBonusMonths;
        var redundancySeveranceMonths = clampedYearsOfService;

        return new(
            employee.Id,
            employee.FullName,
            employee.Email,
            employee.PhoneNumber,
            employee.EmployeeId,
            employee.DateOfBirth,
            employee.FirstEmploymentDate,
            employee.JobLevel,
            annualLeaveEntitlementDays,
            clampedAge,
            clampedYearsOfService,
            mandatoryRetirementDate,
            isEligibleForVoluntaryRetirement,
            isAtOrAboveMandatoryRetirementAge,
            isEligibleForGoldenHandshake,
            isEligibleForLongServiceAward,
            noticePeriodMonths,
            serviceGratuityMonthsPerCompletedYear,
            serviceGratuityTotalMonths,
            redundancySeveranceMonths,
            employee.DepartmentId,
            employee.Department?.Name ?? string.Empty,
            employee.JobTitle,
            employee.EmploymentType,
            employee.AnnualLeaveBalanceDays);
    }

    private static int GetAnnualLeaveEntitlementDays(int jobLevel) => jobLevel >= 10 ? 36 : 30;

    private static int GetNoticePeriodMonths(int yearsOfService) => yearsOfService switch
    {
        >= 10 => 3,
        >= 5 => 2,
        >= 1 => 1,
        _ => 0
    };

    private static decimal GetServiceGratuityMonthsPerCompletedYear(int yearsOfService) => yearsOfService switch
    {
        < 4 => 0m,
        < 10 => 1.5m,
        < 15 => 2m,
        < 20 => 2.5m,
        _ => 3m
    };
}
