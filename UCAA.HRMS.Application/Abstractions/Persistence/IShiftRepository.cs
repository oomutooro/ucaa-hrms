using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IShiftRepository
{
    Task<List<ShiftAssignment>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<AttendanceRecord>> ListAttendanceAsync(CancellationToken cancellationToken = default);
    Task<AttendanceRecord?> GetAttendanceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendanceRecord?> GetOpenAttendanceAsync(Guid employeeId, DateOnly attendanceDate, CancellationToken cancellationToken = default);
    Task<ShiftAssignment?> GetEmployeeShiftForDateAsync(Guid employeeId, DateOnly shiftDate, CancellationToken cancellationToken = default);
    Task<int> CountUpcomingAsync(DateOnly fromDate, CancellationToken cancellationToken = default);
    Task<bool> ExistsForDateAndShiftAsync(DateOnly shiftDate, string shiftCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsForEmployeeAndDateAsync(Guid employeeId, DateOnly shiftDate, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(ShiftAssignment assignment, CancellationToken cancellationToken = default);
    Task AddAttendanceAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default);
    void UpdateAttendance(AttendanceRecord attendanceRecord);
}
