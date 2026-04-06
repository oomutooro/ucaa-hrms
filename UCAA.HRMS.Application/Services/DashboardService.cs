using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IEmployeeRepository _employees;
    private readonly ILeaveRequestRepository _leaveRequests;
    private readonly IShiftRepository _shifts;

    public DashboardService(IEmployeeRepository employees, ILeaveRequestRepository leaveRequests, IShiftRepository shifts)
    {
        _employees = employees;
        _leaveRequests = leaveRequests;
        _shifts = shifts;
    }

    public async Task<DashboardMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        var totalEmployees = (await _employees.ListAsync(cancellationToken)).Count;
        var employeesOnLeave = await _leaveRequests.CountEmployeesOnLeaveAsync(DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
        var upcomingShifts = await _shifts.CountUpcomingAsync(DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        return new DashboardMetricsDto(totalEmployees, employeesOnLeave, upcomingShifts);
    }
}
