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

public sealed record GenerateShiftRotationRequest(DateOnly StartDate, int Days, List<Guid> EmployeeIds);

public sealed record ClockInAttendanceRequest(Guid EmployeeId, DateOnly AttendanceDate, TimeOnly? CheckInTime, string? Notes);

public sealed record ClockOutAttendanceRequest(TimeOnly? CheckOutTime, string? Notes);

public sealed record AttendanceRecordDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid? ShiftAssignmentId,
    DateOnly AttendanceDate,
    ShiftType? ShiftType,
    AttendanceStatus Status,
    TimeOnly CheckInTime,
    TimeOnly? CheckOutTime,
    decimal HoursWorked,
    bool IsOpen,
    string? Notes);

public sealed record EmployeeAttendanceRollupDto(
    Guid EmployeeId,
    string EmployeeName,
    int PresentDays,
    int LateDays,
    decimal TotalHoursWorked);

public sealed record AttendanceSummaryDto(
    int ScheduledToday,
    int CheckedInToday,
    int LateToday,
    int PendingClockOuts,
    decimal TotalHoursToday,
    List<ShiftAssignmentDto> UpcomingAssignments,
    List<AttendanceRecordDto> RecentAttendance,
    List<EmployeeAttendanceRollupDto> MonthlyRollup);
