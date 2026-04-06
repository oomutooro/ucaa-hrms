using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class OnboardingController : ControllerBase
{
    private readonly IOnboardingService _svc;

    public OnboardingController(IOnboardingService svc) => _svc = svc;

    // ── Templates ─────────────────────────────────────────────────────────────

    [HttpGet("templates")]
    public Task<List<OnboardingTemplateDto>> GetTemplates(CancellationToken ct) =>
        _svc.ListTemplatesAsync(ct);

    [HttpGet("templates/{id:guid}")]
    public Task<OnboardingTemplateDetailDto> GetTemplate(Guid id, CancellationToken ct) =>
        _svc.GetTemplateAsync(id, ct);

    [HttpPost("templates")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<OnboardingTemplateDto> CreateTemplate([FromBody] CreateOnboardingTemplateRequest request, CancellationToken ct) =>
        _svc.CreateTemplateAsync(request, ct);

    [HttpDelete("templates/{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public async Task<IActionResult> DeleteTemplate(Guid id, CancellationToken ct)
    {
        await _svc.DeleteTemplateAsync(id, ct);
        return NoContent();
    }

    [HttpPost("templates/{templateId:guid}/tasks")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<OnboardingTemplateTaskDto> AddTemplateTask(Guid templateId, [FromBody] AddTemplateTaskRequest request, CancellationToken ct) =>
        _svc.AddTemplateTaskAsync(templateId, request, ct);

    [HttpDelete("template-tasks/{taskId:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public async Task<IActionResult> RemoveTemplateTask(Guid taskId, CancellationToken ct)
    {
        await _svc.RemoveTemplateTaskAsync(taskId, ct);
        return NoContent();
    }

    // ── Employee onboardings ──────────────────────────────────────────────────

    [HttpGet("records")]
    public Task<List<EmployeeOnboardingDto>> GetOnboardings(CancellationToken ct) =>
        _svc.ListOnboardingsAsync(ct);

    [HttpGet("records/{id:guid}")]
    public Task<EmployeeOnboardingDetailDto> GetOnboarding(Guid id, CancellationToken ct) =>
        _svc.GetOnboardingAsync(id, ct);

    [HttpPost("records")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<EmployeeOnboardingDto> CreateOnboarding([FromBody] CreateEmployeeOnboardingRequest request, CancellationToken ct) =>
        _svc.CreateOnboardingAsync(request, ct);

    [HttpPatch("records/{id:guid}/status")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<EmployeeOnboardingDto> UpdateOnboardingStatus(Guid id, [FromBody] UpdateOnboardingStatusRequest request, CancellationToken ct) =>
        _svc.UpdateOnboardingStatusAsync(id, request, ct);

    // ── Checklist items ───────────────────────────────────────────────────────

    [HttpPatch("items/{itemId:guid}")]
    public Task<OnboardingItemDto> UpdateItem(Guid itemId, [FromBody] UpdateOnboardingItemRequest request, CancellationToken ct) =>
        _svc.UpdateItemAsync(itemId, request, ct);
}
