using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IShiftRepository
{
    Task<List<ShiftAssignment>> ListAsync(CancellationToken cancellationToken = default);
    Task<int> CountUpcomingAsync(DateOnly fromDate, CancellationToken cancellationToken = default);
    Task<bool> ExistsForDateAndShiftAsync(DateOnly shiftDate, string shiftCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(ShiftAssignment assignment, CancellationToken cancellationToken = default);
}
