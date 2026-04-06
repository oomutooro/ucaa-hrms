using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class JobArchitectureController : ControllerBase
{
    private readonly IJobArchitectureService _svc;

    public JobArchitectureController(IJobArchitectureService svc) => _svc = svc;

    // ── Job Grades ────────────────────────────────────────────────────────────

    [HttpGet("grades")]
    public Task<List<JobGradeDto>> GetGrades(CancellationToken ct) =>
        _svc.ListGradesAsync(ct);

    [HttpPost("grades")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobGradeDto> CreateGrade([FromBody] CreateJobGradeRequest request, CancellationToken ct) =>
        _svc.CreateGradeAsync(request, ct);

    [HttpPut("grades/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobGradeDto> UpdateGrade(Guid id, [FromBody] UpdateJobGradeRequest request, CancellationToken ct) =>
        _svc.UpdateGradeAsync(id, request, ct);

    [HttpDelete("grades/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public async Task<IActionResult> DeleteGrade(Guid id, CancellationToken ct)
    {
        await _svc.DeleteGradeAsync(id, ct);
        return NoContent();
    }

    // ── Job Descriptions ──────────────────────────────────────────────────────

    [HttpGet("job-descriptions")]
    public Task<List<JobDescriptionDto>> GetJobDescriptions(CancellationToken ct) =>
        _svc.ListJobDescriptionsAsync(ct);

    [HttpPost("job-descriptions")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobDescriptionDto> CreateJobDescription([FromBody] CreateJobDescriptionRequest request, CancellationToken ct) =>
        _svc.CreateJobDescriptionAsync(request, ct);

    [HttpPut("job-descriptions/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobDescriptionDto> UpdateJobDescription(Guid id, [FromBody] UpdateJobDescriptionRequest request, CancellationToken ct) =>
        _svc.UpdateJobDescriptionAsync(id, request, ct);

    [HttpDelete("job-descriptions/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public async Task<IActionResult> DeleteJobDescription(Guid id, CancellationToken ct)
    {
        await _svc.DeleteJobDescriptionAsync(id, ct);
        return NoContent();
    }

    // ── Positions ─────────────────────────────────────────────────────────────

    [HttpGet("positions")]
    public Task<List<PositionDto>> GetPositions(CancellationToken ct) =>
        _svc.ListPositionsAsync(ct);

    [HttpPost("positions")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<PositionDto> CreatePosition([FromBody] CreatePositionRequest request, CancellationToken ct) =>
        _svc.CreatePositionAsync(request, ct);

    [HttpPut("positions/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<PositionDto> UpdatePosition(Guid id, [FromBody] UpdatePositionRequest request, CancellationToken ct) =>
        _svc.UpdatePositionAsync(id, request, ct);

    [HttpDelete("positions/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public async Task<IActionResult> DeletePosition(Guid id, CancellationToken ct)
    {
        await _svc.DeletePositionAsync(id, ct);
        return NoContent();
    }
}
