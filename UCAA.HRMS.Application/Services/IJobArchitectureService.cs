using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IJobArchitectureService
{
    // Job Grades
    Task<List<JobGradeDto>> ListGradesAsync(CancellationToken ct = default);
    Task<JobGradeDto> CreateGradeAsync(CreateJobGradeRequest request, CancellationToken ct = default);
    Task<JobGradeDto> UpdateGradeAsync(Guid id, UpdateJobGradeRequest request, CancellationToken ct = default);
    Task DeleteGradeAsync(Guid id, CancellationToken ct = default);

    // Job Descriptions
    Task<List<JobDescriptionDto>> ListJobDescriptionsAsync(CancellationToken ct = default);
    Task<JobDescriptionDto> CreateJobDescriptionAsync(CreateJobDescriptionRequest request, CancellationToken ct = default);
    Task<JobDescriptionDto> UpdateJobDescriptionAsync(Guid id, UpdateJobDescriptionRequest request, CancellationToken ct = default);
    Task DeleteJobDescriptionAsync(Guid id, CancellationToken ct = default);

    // Positions
    Task<List<PositionDto>> ListPositionsAsync(CancellationToken ct = default);
    Task<PositionDto> CreatePositionAsync(CreatePositionRequest request, CancellationToken ct = default);
    Task<PositionDto> UpdatePositionAsync(Guid id, UpdatePositionRequest request, CancellationToken ct = default);
    Task DeletePositionAsync(Guid id, CancellationToken ct = default);
}
