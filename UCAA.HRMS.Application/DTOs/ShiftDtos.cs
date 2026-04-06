using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record ShiftAssignmentDto(
    Guid Id,
    DateOnly ShiftDate,
    ShiftType ShiftType,
    Guid? EmployeeId,
    string? EmployeeName,
    string ShiftCode);

public sealed record AssignShiftRequest(Guid? EmployeeId, DateOnly ShiftDate, ShiftType ShiftType);
