using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IRecruitmentRepository
{
    // Requisitions
    Task<List<JobRequisition>> ListRequisitionsAsync(CancellationToken ct = default);
    Task<JobRequisition?> GetRequisitionByIdAsync(Guid id, CancellationToken ct = default);
    Task AddRequisitionAsync(JobRequisition requisition, CancellationToken ct = default);
    void UpdateRequisition(JobRequisition requisition);
    void RemoveRequisition(JobRequisition requisition);
    Task<bool> RequisitionHasApplicationsAsync(Guid requisitionId, CancellationToken ct = default);
    Task<int> CountRequisitionsInYearAsync(int year, CancellationToken ct = default);

    // Applications
    Task<List<JobApplication>> ListApplicationsAsync(Guid? requisitionId = null, CancellationToken ct = default);
    Task<JobApplication?> GetApplicationByIdAsync(Guid id, CancellationToken ct = default);
    Task AddApplicationAsync(JobApplication application, CancellationToken ct = default);
    void UpdateApplication(JobApplication application);
    void RemoveApplication(JobApplication application);
}
