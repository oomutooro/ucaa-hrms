using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public sealed class ShiftService : IShiftService
{
    private static readonly TimeOnly DefaultDayStart = new(8, 0);
    private static readonly TimeOnly DefaultNightStart = new(20, 0);
    private static readonly TimeSpan LateGracePeriod = TimeSpan.FromMinutes(15);

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

    public async Task<AttendanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var shifts = await _shifts.ListAsync(cancellationToken);
        var attendance = await _shifts.ListAttendanceAsync(cancellationToken);

        var todayAttendance = attendance.Where(a => a.AttendanceDate == today).ToList();
        var monthlyAttendance = attendance.Where(a => a.AttendanceDate >= monthStart).ToList();

        var monthlyRollup = monthlyAttendance
            .GroupBy(a => new { a.EmployeeId, EmployeeName = a.Employee?.FullName ?? string.Empty })
            .Select(group => new EmployeeAttendanceRollupDto(
                group.Key.EmployeeId,
                group.Key.EmployeeName,
                group.Count(x => x.Status == Domain.Enums.AttendanceStatus.Present || x.Status == Domain.Enums.AttendanceStatus.Late),
                group.Count(x => x.Status == Domain.Enums.AttendanceStatus.Late),
                group.Sum(x => x.HoursWorked)))
            .OrderByDescending(x => x.TotalHoursWorked)
            .ThenBy(x => x.EmployeeName)
            .ToList();

        return new AttendanceSummaryDto(
            shifts.Count(s => s.ShiftDate == today && s.ShiftType != ShiftType.Off && s.EmployeeId.HasValue),
            todayAttendance.Count,
            todayAttendance.Count(a => a.Status == Domain.Enums.AttendanceStatus.Late),
            todayAttendance.Count(a => a.CheckOutTime == null),
            todayAttendance.Sum(a => a.HoursWorked),
            shifts.Where(s => s.ShiftDate >= today)
                .OrderBy(s => s.ShiftDate)
                .ThenBy(s => s.Employee?.FullName ?? string.Empty)
                .Take(10)
                .Select(Map)
                .ToList(),
            attendance.Take(12).Select(MapAttendance).ToList(),
            monthlyRollup);
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

            if (await _shifts.ExistsForEmployeeAndDateAsync(request.EmployeeId.Value, request.ShiftDate, cancellationToken: cancellationToken))
            {
                throw new AppException("Employee already has a shift assigned for this date.");
            }
        }

        var code = request.EmployeeId.HasValue
            ? $"{request.ShiftDate:yyyyMMdd}-{request.ShiftType}-{request.EmployeeId.Value.ToString()[..8]}"
            : $"{request.ShiftDate:yyyyMMdd}-{request.ShiftType}-OPEN";
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

    public async Task<AttendanceRecordDto> ClockInAsync(ClockInAttendanceRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new AppException("Employee not found.", 404);

        if (await _shifts.GetOpenAttendanceAsync(request.EmployeeId, request.AttendanceDate, cancellationToken) is not null)
        {
            throw new AppException("Employee already has an open attendance record for this date.");
        }

        var assignedShift = await _shifts.GetEmployeeShiftForDateAsync(request.EmployeeId, request.AttendanceDate, cancellationToken);
        if (assignedShift?.ShiftType == ShiftType.Off)
        {
            throw new AppException("Employee is scheduled off duty for this date.");
        }

        var checkInTime = request.CheckInTime ?? TimeOnly.FromDateTime(DateTime.Now);
        var scheduledStart = GetScheduledStartTime(assignedShift?.ShiftType);
        var status = checkInTime > scheduledStart.Add(LateGracePeriod)
            ? Domain.Enums.AttendanceStatus.Late
            : Domain.Enums.AttendanceStatus.Present;

        var attendanceRecord = new AttendanceRecord
        {
            EmployeeId = request.EmployeeId,
            Employee = employee,
            ShiftAssignmentId = assignedShift?.Id,
            ShiftAssignment = assignedShift,
            AttendanceDate = request.AttendanceDate,
            CheckInTime = checkInTime,
            Status = status,
            Notes = request.Notes,
            HoursWorked = 0m
        };

        await _shifts.AddAttendanceAsync(attendanceRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapAttendance(attendanceRecord);
    }

    public async Task<AttendanceRecordDto> ClockOutAsync(Guid attendanceId, ClockOutAttendanceRequest request, CancellationToken cancellationToken = default)
    {
        var attendanceRecord = await _shifts.GetAttendanceByIdAsync(attendanceId, cancellationToken)
            ?? throw new AppException("Attendance record not found.", 404);

        if (attendanceRecord.CheckOutTime.HasValue)
        {
            throw new AppException("Attendance record has already been closed.");
        }

        var checkOutTime = request.CheckOutTime ?? TimeOnly.FromDateTime(DateTime.Now);
        var workedMinutes = CalculateWorkedMinutes(attendanceRecord.CheckInTime, checkOutTime);
        if (workedMinutes <= 0)
        {
            throw new AppException("Check-out time must be after check-in time.");
        }

        attendanceRecord.CheckOutTime = checkOutTime;
        attendanceRecord.HoursWorked = Math.Round(workedMinutes / 60m, 2, MidpointRounding.AwayFromZero);
        attendanceRecord.Notes = string.IsNullOrWhiteSpace(request.Notes)
            ? attendanceRecord.Notes
            : request.Notes;
        attendanceRecord.UpdatedAtUtc = DateTime.UtcNow;

        _shifts.UpdateAttendance(attendanceRecord);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapAttendance(attendanceRecord);
    }

    private static ShiftAssignmentDto Map(ShiftAssignment shift) =>
        new(shift.Id, shift.ShiftDate, shift.ShiftType, shift.EmployeeId, shift.Employee?.FullName, shift.ShiftCode);

    private static AttendanceRecordDto MapAttendance(AttendanceRecord attendanceRecord) =>
        new(
            attendanceRecord.Id,
            attendanceRecord.EmployeeId,
            attendanceRecord.Employee?.FullName ?? string.Empty,
            attendanceRecord.ShiftAssignmentId,
            attendanceRecord.AttendanceDate,
            attendanceRecord.ShiftAssignment?.ShiftType,
            attendanceRecord.Status,
            attendanceRecord.CheckInTime,
            attendanceRecord.CheckOutTime,
            attendanceRecord.HoursWorked,
            attendanceRecord.CheckOutTime == null,
            attendanceRecord.Notes);

    private static TimeOnly GetScheduledStartTime(ShiftType? shiftType) => shiftType switch
    {
        ShiftType.Night => DefaultNightStart,
        _ => DefaultDayStart
    };

    private static int CalculateWorkedMinutes(TimeOnly checkInTime, TimeOnly checkOutTime)
    {
        var startMinutes = checkInTime.Hour * 60 + checkInTime.Minute;
        var endMinutes = checkOutTime.Hour * 60 + checkOutTime.Minute;

        if (endMinutes < startMinutes)
        {
            endMinutes += 24 * 60;
        }

        return endMinutes - startMinutes;
    }
}
