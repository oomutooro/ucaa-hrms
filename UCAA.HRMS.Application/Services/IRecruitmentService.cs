using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IRecruitmentService
{
    // Requisitions
    Task<List<JobRequisitionDto>> ListRequisitionsAsync(CancellationToken ct = default);
    Task<JobRequisitionDto> CreateRequisitionAsync(CreateJobRequisitionRequest request, CancellationToken ct = default);
    Task<JobRequisitionDto> UpdateRequisitionStatusAsync(Guid id, UpdateRequisitionStatusRequest request, CancellationToken ct = default);
    Task DeleteRequisitionAsync(Guid id, CancellationToken ct = default);

    // Applications
    Task<List<JobApplicationDto>> ListApplicationsAsync(Guid? requisitionId = null, CancellationToken ct = default);
    Task<JobApplicationDto> CreateApplicationAsync(CreateJobApplicationRequest request, CancellationToken ct = default);
    Task<JobApplicationDto> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request, CancellationToken ct = default);
    Task DeleteApplicationAsync(Guid id, CancellationToken ct = default);
}
