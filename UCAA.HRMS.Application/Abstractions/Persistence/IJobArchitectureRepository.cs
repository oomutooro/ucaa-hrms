using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IJobArchitectureRepository
{
    // Job Grades
    Task<List<JobGrade>> ListGradesAsync(CancellationToken ct = default);
    Task<JobGrade?> GetGradeByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> GradeCodeExistsAsync(string gradeCode, Guid? excludeId = null, CancellationToken ct = default);
    Task AddGradeAsync(JobGrade grade, CancellationToken ct = default);
    void UpdateGrade(JobGrade grade);
    void RemoveGrade(JobGrade grade);
    Task<bool> GradeHasJobDescriptionsAsync(Guid gradeId, CancellationToken ct = default);

    // Job Descriptions
    Task<List<JobDescription>> ListJobDescriptionsAsync(CancellationToken ct = default);
    Task<JobDescription?> GetJobDescriptionByIdAsync(Guid id, CancellationToken ct = default);
    Task AddJobDescriptionAsync(JobDescription jd, CancellationToken ct = default);
    void UpdateJobDescription(JobDescription jd);
    void RemoveJobDescription(JobDescription jd);
    Task<bool> JobDescriptionHasPositionsAsync(Guid jobDescriptionId, CancellationToken ct = default);

    // Positions
    Task<List<Position>> ListPositionsAsync(CancellationToken ct = default);
    Task<Position?> GetPositionByIdAsync(Guid id, CancellationToken ct = default);
    Task AddPositionAsync(Position position, CancellationToken ct = default);
    void UpdatePosition(Position position);
    void RemovePosition(Position position);
}
