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

    [HttpPost("assign")]
    [Authorize(Roles = "Admin,HR Manager,Supervisor")]
    public Task<ShiftAssignmentDto> Assign([FromBody] AssignShiftRequest request, CancellationToken cancellationToken) =>
        _shiftService.AssignAsync(request, cancellationToken);

    [HttpPost("rotation")]
    [Authorize(Roles = "Admin,HR Manager,Supervisor")]
    public Task<List<ShiftAssignmentDto>> GenerateRotation([FromQuery] DateOnly startDate, [FromQuery] int days, [FromBody] List<Guid> employeeIds, CancellationToken cancellationToken) =>
        _shiftService.GenerateRotationAsync(startDate, days, employeeIds, cancellationToken);
}
