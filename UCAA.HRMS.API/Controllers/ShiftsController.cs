using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ShiftsController : ControllerBase
{
    private readonly IShiftService _shiftService;

    public ShiftsController(IShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    [HttpGet]
    public Task<List<ShiftAssignmentDto>> GetAll(CancellationToken cancellationToken) =>
        _shiftService.ListAsync(cancellationToken);

    [HttpGet("summary")]
    public Task<AttendanceSummaryDto> GetSummary(CancellationToken cancellationToken) =>
        _shiftService.GetSummaryAsync(cancellationToken);

    [HttpPost("assign")]
    [Authorize(Roles = "Admin,HR Manager,Supervisor")]
    public Task<ShiftAssignmentDto> Assign([FromBody] AssignShiftRequest request, CancellationToken cancellationToken) =>
        _shiftService.AssignAsync(request, cancellationToken);

    [HttpPost("rotation")]
    [Authorize(Roles = "Admin,HR Manager,Supervisor")]
    public Task<List<ShiftAssignmentDto>> GenerateRotation([FromBody] GenerateShiftRotationRequest request, CancellationToken cancellationToken) =>
        _shiftService.GenerateRotationAsync(request.StartDate, request.Days, request.EmployeeIds, cancellationToken);

    [HttpPost("attendance/clock-in")]
    public Task<AttendanceRecordDto> ClockIn([FromBody] ClockInAttendanceRequest request, CancellationToken cancellationToken) =>
        _shiftService.ClockInAsync(request, cancellationToken);

    [HttpPost("attendance/{attendanceId:guid}/clock-out")]
    public Task<AttendanceRecordDto> ClockOut(Guid attendanceId, [FromBody] ClockOutAttendanceRequest request, CancellationToken cancellationToken) =>
        _shiftService.ClockOutAsync(attendanceId, request, cancellationToken);
}
