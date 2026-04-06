using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class ShiftRepository : IShiftRepository
{
    private readonly ApplicationDbContext _db;

    public ShiftRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<ShiftAssignment>> ListAsync(CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.Include(s => s.Employee).OrderBy(s => s.ShiftDate).ToListAsync(cancellationToken);

    public Task<List<AttendanceRecord>> ListAttendanceAsync(CancellationToken cancellationToken = default) =>
        _db.AttendanceRecords
            .Include(a => a.Employee)
            .Include(a => a.ShiftAssignment)
            .OrderByDescending(a => a.AttendanceDate)
            .ThenByDescending(a => a.CheckInTime)
            .ToListAsync(cancellationToken);

    public Task<AttendanceRecord?> GetAttendanceByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.AttendanceRecords
            .Include(a => a.Employee)
            .Include(a => a.ShiftAssignment)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<AttendanceRecord?> GetOpenAttendanceAsync(Guid employeeId, DateOnly attendanceDate, CancellationToken cancellationToken = default) =>
        _db.AttendanceRecords
            .Include(a => a.Employee)
            .Include(a => a.ShiftAssignment)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == attendanceDate && a.CheckOutTime == null, cancellationToken);

    public Task<ShiftAssignment?> GetEmployeeShiftForDateAsync(Guid employeeId, DateOnly shiftDate, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.ShiftDate == shiftDate, cancellationToken);

    public Task<int> CountUpcomingAsync(DateOnly fromDate, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.CountAsync(s => s.ShiftDate >= fromDate, cancellationToken);

    public Task<bool> ExistsForDateAndShiftAsync(DateOnly shiftDate, string shiftCode, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.AnyAsync(s =>
            s.ShiftDate == shiftDate &&
            s.ShiftCode == shiftCode &&
            (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);

    public Task<bool> ExistsForEmployeeAndDateAsync(Guid employeeId, DateOnly shiftDate, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.AnyAsync(s =>
            s.EmployeeId == employeeId &&
            s.ShiftDate == shiftDate &&
            (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(ShiftAssignment assignment, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.AddAsync(assignment, cancellationToken).AsTask();

    public Task AddAttendanceAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default) =>
        _db.AttendanceRecords.AddAsync(attendanceRecord, cancellationToken).AsTask();

    public void UpdateAttendance(AttendanceRecord attendanceRecord) => _db.AttendanceRecords.Update(attendanceRecord);
}
