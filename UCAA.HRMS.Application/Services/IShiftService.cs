using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IShiftService
{
    Task<List<ShiftAssignmentDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<AttendanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<ShiftAssignmentDto> AssignAsync(AssignShiftRequest request, CancellationToken cancellationToken = default);
    Task<List<ShiftAssignmentDto>> GenerateRotationAsync(DateOnly startDate, int days, List<Guid> employeeIds, CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto> ClockInAsync(ClockInAttendanceRequest request, CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto> ClockOutAsync(Guid attendanceId, ClockOutAttendanceRequest request, CancellationToken cancellationToken = default);
}
