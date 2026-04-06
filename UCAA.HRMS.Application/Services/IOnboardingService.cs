using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IOnboardingService
{
    // Templates
    Task<List<OnboardingTemplateDto>> ListTemplatesAsync(CancellationToken ct = default);
    Task<OnboardingTemplateDetailDto> GetTemplateAsync(Guid id, CancellationToken ct = default);
    Task<OnboardingTemplateDto> CreateTemplateAsync(CreateOnboardingTemplateRequest request, CancellationToken ct = default);
    Task DeleteTemplateAsync(Guid id, CancellationToken ct = default);
    Task<OnboardingTemplateTaskDto> AddTemplateTaskAsync(Guid templateId, AddTemplateTaskRequest request, CancellationToken ct = default);
    Task RemoveTemplateTaskAsync(Guid taskId, CancellationToken ct = default);

    // Employee onboardings
    Task<List<EmployeeOnboardingDto>> ListOnboardingsAsync(CancellationToken ct = default);
    Task<EmployeeOnboardingDetailDto> GetOnboardingAsync(Guid id, CancellationToken ct = default);
    Task<EmployeeOnboardingDto> CreateOnboardingAsync(CreateEmployeeOnboardingRequest request, CancellationToken ct = default);
    Task<EmployeeOnboardingDto> UpdateOnboardingStatusAsync(Guid id, UpdateOnboardingStatusRequest request, CancellationToken ct = default);

    // Checklist items
    Task<OnboardingItemDto> UpdateItemAsync(Guid itemId, UpdateOnboardingItemRequest request, CancellationToken ct = default);
}
