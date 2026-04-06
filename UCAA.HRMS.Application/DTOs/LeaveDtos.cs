using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    LeaveType LeaveType,
    LeaveStatus Status,
    DateOnly StartDate,
    DateOnly EndDate,
    int RequestedDays,
    int? SickLeavePayPercent,
    string Reason,
    string? ReviewerComment);

public sealed record ApplyLeaveRequest(
    Guid EmployeeId,
    LeaveType LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason);

public sealed record ReviewLeaveRequest(LeaveStatus Status, string? ReviewerComment);
