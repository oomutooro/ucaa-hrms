using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public sealed class ShiftService : IShiftService
{
    private static readonly ShiftType[] Rotation =
    {
        ShiftType.Day,
        ShiftType.Night,
        ShiftType.Off,
        ShiftType.Off
    };

    private readonly IShiftRepository _shifts;
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _unitOfWork;

    public ShiftService(IShiftRepository shifts, IEmployeeRepository employees, IUnitOfWork unitOfWork)
    {
        _shifts = shifts;
        _employees = employees;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ShiftAssignmentDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _shifts.ListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<ShiftAssignmentDto> AssignAsync(AssignShiftRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EmployeeId.HasValue)
        {
            var employee = await _employees.GetByIdAsync(request.EmployeeId.Value, cancellationToken);
            if (employee is null)
            {
                throw new AppException("Employee not found.", 404);
            }
        }

        var code = $"{request.ShiftDate:yyyyMMdd}-{request.ShiftType}";
        if (await _shifts.ExistsForDateAndShiftAsync(request.ShiftDate, code, cancellationToken: cancellationToken))
        {
            throw new AppException("Shift slot for this date/type is already assigned.");
        }

        var assignment = new ShiftAssignment
        {
            EmployeeId = request.EmployeeId,
            ShiftDate = request.ShiftDate,
            ShiftType = request.ShiftType,
            ShiftCode = code
        };

        await _shifts.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(assignment);
    }

    public async Task<List<ShiftAssignmentDto>> GenerateRotationAsync(DateOnly startDate, int days, List<Guid> employeeIds, CancellationToken cancellationToken = default)
    {
        if (days <= 0)
        {
            throw new AppException("Days must be greater than zero.");
        }

        var generated = new List<ShiftAssignmentDto>();
        for (var i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var shiftType = Rotation[i % Rotation.Length];
            Guid? employeeId = employeeIds.Count == 0 ? null : employeeIds[i % employeeIds.Count];

            var assignment = await AssignAsync(new AssignShiftRequest(employeeId, date, shiftType), cancellationToken);
            generated.Add(assignment);
        }

        return generated;
    }

    private static ShiftAssignmentDto Map(ShiftAssignment shift) =>
        new(shift.Id, shift.ShiftDate, shift.ShiftType, shift.EmployeeId, shift.Employee?.FullName, shift.ShiftCode);
}
