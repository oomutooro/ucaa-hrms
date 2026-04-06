using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class BenefitsController : ControllerBase
{
    private readonly IBenefitService _svc;

    public BenefitsController(IBenefitService svc) => _svc = svc;

    [HttpGet("plans")]
    public Task<List<BenefitPlanDto>> GetPlans(CancellationToken ct) =>
        _svc.ListPlansAsync(ct);

    [HttpPost("plans")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<BenefitPlanDto> CreatePlan([FromBody] CreateBenefitPlanRequest request, CancellationToken ct) =>
        _svc.CreatePlanAsync(request, ct);

    [HttpPut("plans/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<BenefitPlanDto> UpdatePlan(Guid id, [FromBody] UpdateBenefitPlanRequest request, CancellationToken ct) =>
        _svc.UpdatePlanAsync(id, request, ct);

    [HttpGet("enrollments")]
    public Task<List<BenefitEnrollmentDto>> GetEnrollments(CancellationToken ct) =>
        _svc.ListEnrollmentsAsync(ct);

    [HttpPost("enrollments")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<BenefitEnrollmentDto> CreateEnrollment([FromBody] CreateBenefitEnrollmentRequest request, CancellationToken ct) =>
        _svc.CreateEnrollmentAsync(request, ct);

    [HttpPatch("enrollments/{id:guid}/status")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<BenefitEnrollmentDto> UpdateEnrollmentStatus(Guid id, [FromBody] UpdateBenefitEnrollmentStatusRequest request, CancellationToken ct) =>
        _svc.UpdateEnrollmentStatusAsync(id, request, ct);

    [HttpGet("summary")]
    public Task<BenefitSummaryDto> GetSummary(CancellationToken ct) =>
        _svc.GetSummaryAsync(ct);
}
