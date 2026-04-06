using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpGet]
    public Task<List<LeaveRequestDto>> GetAll(CancellationToken cancellationToken) =>
        _leaveService.ListAsync(cancellationToken);

    [HttpPost("apply")]
    public Task<LeaveRequestDto> Apply([FromBody] ApplyLeaveRequest request, CancellationToken cancellationToken) =>
        _leaveService.ApplyAsync(request, cancellationToken);

    [HttpPost("{id:guid}/review")]
    [Authorize(Roles = "Admin,HR Manager,Supervisor")]
    public Task<LeaveRequestDto> Review(Guid id, [FromBody] ReviewLeaveRequest request, CancellationToken cancellationToken) =>
        _leaveService.ReviewAsync(id, request, cancellationToken);
}
