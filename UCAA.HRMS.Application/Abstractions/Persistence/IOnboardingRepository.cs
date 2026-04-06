using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IOnboardingRepository
{
    // Templates
    Task<List<OnboardingTemplate>> ListTemplatesAsync(CancellationToken ct = default);
    Task<OnboardingTemplate?> GetTemplateByIdAsync(Guid id, CancellationToken ct = default);
    Task AddTemplateAsync(OnboardingTemplate template, CancellationToken ct = default);
    void RemoveTemplate(OnboardingTemplate template);

    // Template tasks
    Task<OnboardingTemplateTask?> GetTemplateTaskByIdAsync(Guid id, CancellationToken ct = default);
    Task AddTemplateTaskAsync(OnboardingTemplateTask task, CancellationToken ct = default);
    void RemoveTemplateTask(OnboardingTemplateTask task);

    // Employee onboardings
    Task<List<EmployeeOnboarding>> ListOnboardingsAsync(CancellationToken ct = default);
    Task<EmployeeOnboarding?> GetOnboardingByIdAsync(Guid id, CancellationToken ct = default);
    Task AddOnboardingAsync(EmployeeOnboarding onboarding, CancellationToken ct = default);
    void UpdateOnboarding(EmployeeOnboarding onboarding);

    // Onboarding items
    Task<OnboardingItem?> GetItemByIdAsync(Guid id, CancellationToken ct = default);
    void UpdateItem(OnboardingItem item);
}
