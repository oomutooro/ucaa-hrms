using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

// ── Template ──────────────────────────────────────────────────────────────────
public sealed record OnboardingTemplateDto(
    Guid Id,
    string Name,
    string Description,
    int TaskCount);

public sealed record OnboardingTemplateDetailDto(
    Guid Id,
    string Name,
    string Description,
    List<OnboardingTemplateTaskDto> Tasks);

public sealed record OnboardingTemplateTaskDto(
    Guid Id,
    string Title,
    string Category,
    bool IsRequired,
    int SortOrder);

public sealed record CreateOnboardingTemplateRequest(
    string Name,
    string Description);

public sealed record AddTemplateTaskRequest(
    string Title,
    string Category,
    bool IsRequired,
    int SortOrder);

// ── Employee Onboarding ───────────────────────────────────────────────────────
public sealed record EmployeeOnboardingDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid? ApplicationId,
    DateOnly StartDate,
    DateOnly TargetCompletionDate,
    OnboardingStatus Status,
    string StatusLabel,
    string? Notes,
    int TotalItems,
    int CompletedItems);

public sealed record EmployeeOnboardingDetailDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid? ApplicationId,
    DateOnly StartDate,
    DateOnly TargetCompletionDate,
    OnboardingStatus Status,
    string StatusLabel,
    string? Notes,
    List<OnboardingItemDto> Items);

public sealed record OnboardingItemDto(
    Guid Id,
    string Title,
    string Category,
    bool IsRequired,
    int SortOrder,
    bool IsCompleted,
    DateTime? CompletedAt,
    string? Notes);

public sealed record CreateEmployeeOnboardingRequest(
    Guid EmployeeId,
    Guid? ApplicationId,
    DateOnly StartDate,
    DateOnly TargetCompletionDate,
    Guid? TemplateId,         // if provided, tasks are copied from this template
    string? Notes);

public sealed record UpdateOnboardingStatusRequest(
    OnboardingStatus Status,
    string? Notes);

public sealed record UpdateOnboardingItemRequest(
    bool IsCompleted,
    string? Notes);
