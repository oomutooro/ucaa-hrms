using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly ApplicationDbContext _db;

    public LeaveRequestRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<LeaveRequest>> ListAsync(CancellationToken cancellationToken = default) =>
        _db.LeaveRequests
            .Include(l => l.Employee)
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.LeaveRequests.Include(l => l.Employee).FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<int> CountEmployeesOnLeaveAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _db.LeaveRequests
            .Where(l => l.Status == LeaveStatus.Approved && l.StartDate <= date && l.EndDate >= date)
            .Select(l => l.EmployeeId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default) =>
        _db.LeaveRequests.AddAsync(leaveRequest, cancellationToken).AsTask();

    public void Update(LeaveRequest leaveRequest) => _db.LeaveRequests.Update(leaveRequest);
}
