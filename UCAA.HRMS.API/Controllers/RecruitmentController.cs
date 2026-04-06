using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RecruitmentController : ControllerBase
{
    private readonly IRecruitmentService _svc;

    public RecruitmentController(IRecruitmentService svc) => _svc = svc;

    // ── Requisitions ──────────────────────────────────────────────────────────

    [HttpGet("requisitions")]
    public Task<List<JobRequisitionDto>> GetRequisitions(CancellationToken ct) =>
        _svc.ListRequisitionsAsync(ct);

    [HttpPost("requisitions")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobRequisitionDto> CreateRequisition([FromBody] CreateJobRequisitionRequest request, CancellationToken ct) =>
        _svc.CreateRequisitionAsync(request, ct);

    [HttpPatch("requisitions/{id:guid}/status")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobRequisitionDto> UpdateRequisitionStatus(Guid id, [FromBody] UpdateRequisitionStatusRequest request, CancellationToken ct) =>
        _svc.UpdateRequisitionStatusAsync(id, request, ct);

    [HttpDelete("requisitions/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public async Task<IActionResult> DeleteRequisition(Guid id, CancellationToken ct)
    {
        await _svc.DeleteRequisitionAsync(id, ct);
        return NoContent();
    }

    // ── Applications ──────────────────────────────────────────────────────────

    [HttpGet("applications")]
    public Task<List<JobApplicationDto>> GetApplications([FromQuery] Guid? requisitionId, CancellationToken ct) =>
        _svc.ListApplicationsAsync(requisitionId, ct);

    [HttpPost("applications")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobApplicationDto> CreateApplication([FromBody] CreateJobApplicationRequest request, CancellationToken ct) =>
        _svc.CreateApplicationAsync(request, ct);

    [HttpPatch("applications/{id:guid}/status")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<JobApplicationDto> UpdateApplicationStatus(Guid id, [FromBody] UpdateApplicationStatusRequest request, CancellationToken ct) =>
        _svc.UpdateApplicationStatusAsync(id, request, ct);

    [HttpDelete("applications/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public async Task<IActionResult> DeleteApplication(Guid id, CancellationToken ct)
    {
        await _svc.DeleteApplicationAsync(id, ct);
        return NoContent();
    }
}
