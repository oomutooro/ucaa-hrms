using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IBenefitService
{
    Task<List<BenefitPlanDto>> ListPlansAsync(CancellationToken ct = default);
    Task<BenefitPlanDto> CreatePlanAsync(CreateBenefitPlanRequest request, CancellationToken ct = default);
    Task<BenefitPlanDto> UpdatePlanAsync(Guid id, UpdateBenefitPlanRequest request, CancellationToken ct = default);

    Task<List<BenefitEnrollmentDto>> ListEnrollmentsAsync(CancellationToken ct = default);
    Task<BenefitEnrollmentDto> CreateEnrollmentAsync(CreateBenefitEnrollmentRequest request, CancellationToken ct = default);
    Task<BenefitEnrollmentDto> UpdateEnrollmentStatusAsync(Guid id, UpdateBenefitEnrollmentStatusRequest request, CancellationToken ct = default);

    Task<BenefitSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}
