using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class OnboardingRepository : IOnboardingRepository
{
    private readonly ApplicationDbContext _db;

    public OnboardingRepository(ApplicationDbContext db) => _db = db;

    // ── Templates ─────────────────────────────────────────────────────────────

    public Task<List<OnboardingTemplate>> ListTemplatesAsync(CancellationToken ct = default) =>
        _db.OnboardingTemplates
            .Include(t => t.Tasks)
            .OrderBy(t => t.Name)
            .ToListAsync(ct);

    public Task<OnboardingTemplate?> GetTemplateByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.OnboardingTemplates
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task AddTemplateAsync(OnboardingTemplate template, CancellationToken ct = default) =>
        _db.OnboardingTemplates.AddAsync(template, ct).AsTask();

    public void RemoveTemplate(OnboardingTemplate template) => _db.OnboardingTemplates.Remove(template);

    // ── Template tasks ────────────────────────────────────────────────────────

    public Task<OnboardingTemplateTask?> GetTemplateTaskByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.OnboardingTemplateTasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task AddTemplateTaskAsync(OnboardingTemplateTask task, CancellationToken ct = default) =>
        _db.OnboardingTemplateTasks.AddAsync(task, ct).AsTask();

    public void RemoveTemplateTask(OnboardingTemplateTask task) => _db.OnboardingTemplateTasks.Remove(task);

    // ── Employee onboardings ──────────────────────────────────────────────────

    public Task<List<EmployeeOnboarding>> ListOnboardingsAsync(CancellationToken ct = default) =>
        _db.EmployeeOnboardings
            .Include(ob => ob.Employee)
            .Include(ob => ob.Items)
            .OrderByDescending(ob => ob.CreatedAtUtc)
            .ToListAsync(ct);

    public Task<EmployeeOnboarding?> GetOnboardingByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.EmployeeOnboardings
            .Include(ob => ob.Employee)
            .Include(ob => ob.Items)
            .FirstOrDefaultAsync(ob => ob.Id == id, ct);

    public Task AddOnboardingAsync(EmployeeOnboarding onboarding, CancellationToken ct = default) =>
        _db.EmployeeOnboardings.AddAsync(onboarding, ct).AsTask();

    public void UpdateOnboarding(EmployeeOnboarding onboarding) => _db.EmployeeOnboardings.Update(onboarding);

    // ── Onboarding items ──────────────────────────────────────────────────────

    public Task<OnboardingItem?> GetItemByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.OnboardingItems.FirstOrDefaultAsync(i => i.Id == id, ct);

    public void UpdateItem(OnboardingItem item) => _db.OnboardingItems.Update(item);
}
