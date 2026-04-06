using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public sealed class OnboardingService : IOnboardingService
{
    private readonly IOnboardingRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public OnboardingService(IOnboardingRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    // ── Templates ─────────────────────────────────────────────────────────────

    public async Task<List<OnboardingTemplateDto>> ListTemplatesAsync(CancellationToken ct = default)
    {
        var templates = await _repo.ListTemplatesAsync(ct);
        return templates.Select(t => new OnboardingTemplateDto(t.Id, t.Name, t.Description, t.Tasks.Count)).ToList();
    }

    public async Task<OnboardingTemplateDetailDto> GetTemplateAsync(Guid id, CancellationToken ct = default)
    {
        var t = await _repo.GetTemplateByIdAsync(id, ct)
            ?? throw new AppException("Template not found.", 404);

        return new OnboardingTemplateDetailDto(
            t.Id, t.Name, t.Description,
            t.Tasks.OrderBy(x => x.SortOrder).Select(MapTemplateTask).ToList());
    }

    public async Task<OnboardingTemplateDto> CreateTemplateAsync(CreateOnboardingTemplateRequest request, CancellationToken ct = default)
    {
        var template = new OnboardingTemplate
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim()
        };

        await _repo.AddTemplateAsync(template, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _repo.GetTemplateByIdAsync(template.Id, ct)!;
        return new OnboardingTemplateDto(saved!.Id, saved.Name, saved.Description, saved.Tasks.Count);
    }

    public async Task DeleteTemplateAsync(Guid id, CancellationToken ct = default)
    {
        var t = await _repo.GetTemplateByIdAsync(id, ct)
            ?? throw new AppException("Template not found.", 404);

        _repo.RemoveTemplate(t);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<OnboardingTemplateTaskDto> AddTemplateTaskAsync(Guid templateId, AddTemplateTaskRequest request, CancellationToken ct = default)
    {
        var template = await _repo.GetTemplateByIdAsync(templateId, ct)
            ?? throw new AppException("Template not found.", 404);

        var task = new OnboardingTemplateTask
        {
            TemplateId = template.Id,
            Title = request.Title.Trim(),
            Category = request.Category.Trim(),
            IsRequired = request.IsRequired,
            SortOrder = request.SortOrder
        };

        await _repo.AddTemplateTaskAsync(task, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapTemplateTask(task);
    }

    public async Task RemoveTemplateTaskAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _repo.GetTemplateTaskByIdAsync(taskId, ct)
            ?? throw new AppException("Template task not found.", 404);

        _repo.RemoveTemplateTask(task);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ── Employee Onboardings ──────────────────────────────────────────────────

    public async Task<List<EmployeeOnboardingDto>> ListOnboardingsAsync(CancellationToken ct = default)
    {
        var items = await _repo.ListOnboardingsAsync(ct);
        return items.Select(MapOnboarding).ToList();
    }

    public async Task<EmployeeOnboardingDetailDto> GetOnboardingAsync(Guid id, CancellationToken ct = default)
    {
        var ob = await _repo.GetOnboardingByIdAsync(id, ct)
            ?? throw new AppException("Onboarding record not found.", 404);

        return new EmployeeOnboardingDetailDto(
            ob.Id, ob.EmployeeId, ob.Employee?.FullName ?? string.Empty,
            ob.ApplicationId, ob.StartDate, ob.TargetCompletionDate,
            ob.Status, StatusLabel(ob.Status), ob.Notes,
            ob.Items.OrderBy(i => i.SortOrder).Select(MapItem).ToList());
    }

    public async Task<EmployeeOnboardingDto> CreateOnboardingAsync(CreateEmployeeOnboardingRequest request, CancellationToken ct = default)
    {
        if (request.TargetCompletionDate < request.StartDate)
            throw new AppException("Target completion date must be on or after start date.");

        var onboarding = new EmployeeOnboarding
        {
            EmployeeId = request.EmployeeId,
            ApplicationId = request.ApplicationId,
            StartDate = request.StartDate,
            TargetCompletionDate = request.TargetCompletionDate,
            Status = OnboardingStatus.NotStarted,
            Notes = request.Notes?.Trim()
        };

        // Copy tasks from template, if provided
        if (request.TemplateId.HasValue)
        {
            var template = await _repo.GetTemplateByIdAsync(request.TemplateId.Value, ct)
                ?? throw new AppException("Onboarding template not found.", 404);

            foreach (var tt in template.Tasks.OrderBy(t => t.SortOrder))
            {
                onboarding.Items.Add(new OnboardingItem
                {
                    Title = tt.Title,
                    Category = tt.Category,
                    IsRequired = tt.IsRequired,
                    SortOrder = tt.SortOrder,
                    IsCompleted = false
                });
            }
        }

        await _repo.AddOnboardingAsync(onboarding, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var saved = await _repo.GetOnboardingByIdAsync(onboarding.Id, ct)!;
        return MapOnboarding(saved!);
    }

    public async Task<EmployeeOnboardingDto> UpdateOnboardingStatusAsync(Guid id, UpdateOnboardingStatusRequest request, CancellationToken ct = default)
    {
        var ob = await _repo.GetOnboardingByIdAsync(id, ct)
            ?? throw new AppException("Onboarding record not found.", 404);

        ob.Status = request.Status;
        ob.Notes = request.Notes?.Trim() ?? ob.Notes;
        ob.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdateOnboarding(ob);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _repo.GetOnboardingByIdAsync(id, ct)!;
        return MapOnboarding(updated!);
    }

    // ── Checklist Items ───────────────────────────────────────────────────────

    public async Task<OnboardingItemDto> UpdateItemAsync(Guid itemId, UpdateOnboardingItemRequest request, CancellationToken ct = default)
    {
        var item = await _repo.GetItemByIdAsync(itemId, ct)
            ?? throw new AppException("Onboarding item not found.", 404);

        item.IsCompleted = request.IsCompleted;
        item.CompletedAt = request.IsCompleted ? DateTime.UtcNow : null;
        item.Notes = request.Notes?.Trim() ?? item.Notes;
        item.UpdatedAtUtc = DateTime.UtcNow;

        _repo.UpdateItem(item);

        // Auto-update parent onboarding status
        var onboarding = await _repo.GetOnboardingByIdAsync(item.OnboardingId, ct);
        if (onboarding is not null)
        {
            var requiredItems = onboarding.Items.Where(i => i.IsRequired).ToList();
            var allRequiredDone = requiredItems.Count > 0 && requiredItems.All(i => i.IsCompleted || i.Id == itemId && request.IsCompleted);
            var anyDone = onboarding.Items.Any(i => i.IsCompleted || i.Id == itemId && request.IsCompleted);

            onboarding.Status = allRequiredDone ? OnboardingStatus.Completed
                : anyDone ? OnboardingStatus.InProgress
                : OnboardingStatus.NotStarted;
            onboarding.UpdatedAtUtc = DateTime.UtcNow;
            _repo.UpdateOnboarding(onboarding);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return MapItem(item);
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    private static string StatusLabel(OnboardingStatus s) => s switch
    {
        OnboardingStatus.NotStarted => "Not Started",
        OnboardingStatus.InProgress => "In Progress",
        OnboardingStatus.Completed  => "Completed",
        _ => s.ToString()
    };

    private static OnboardingTemplateTaskDto MapTemplateTask(OnboardingTemplateTask t) =>
        new(t.Id, t.Title, t.Category, t.IsRequired, t.SortOrder);

    private static OnboardingItemDto MapItem(OnboardingItem i) =>
        new(i.Id, i.Title, i.Category, i.IsRequired, i.SortOrder, i.IsCompleted, i.CompletedAt, i.Notes);

    private static EmployeeOnboardingDto MapOnboarding(EmployeeOnboarding ob) =>
        new(ob.Id, ob.EmployeeId, ob.Employee?.FullName ?? string.Empty,
            ob.ApplicationId, ob.StartDate, ob.TargetCompletionDate,
            ob.Status, StatusLabel(ob.Status), ob.Notes,
            ob.Items.Count, ob.Items.Count(i => i.IsCompleted));
}
