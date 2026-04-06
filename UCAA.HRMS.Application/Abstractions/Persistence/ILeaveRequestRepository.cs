using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface ILeaveRequestRepository
{
    Task<List<LeaveRequest>> ListAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountEmployeesOnLeaveAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);
    void Update(LeaveRequest leaveRequest);
}
