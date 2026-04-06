using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface ILeaveService
{
    Task<List<LeaveRequestDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequestDto> ApplyAsync(ApplyLeaveRequest request, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto> ReviewAsync(Guid id, ReviewLeaveRequest request, CancellationToken cancellationToken = default);
}
