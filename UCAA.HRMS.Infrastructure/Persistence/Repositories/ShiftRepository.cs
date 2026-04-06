using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class ShiftRepository : IShiftRepository
{
    private readonly ApplicationDbContext _db;

    public ShiftRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<ShiftAssignment>> ListAsync(CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.Include(s => s.Employee).OrderBy(s => s.ShiftDate).ToListAsync(cancellationToken);

    public Task<int> CountUpcomingAsync(DateOnly fromDate, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.CountAsync(s => s.ShiftDate >= fromDate, cancellationToken);

    public Task<bool> ExistsForDateAndShiftAsync(DateOnly shiftDate, string shiftCode, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.AnyAsync(s =>
            s.ShiftDate == shiftDate &&
            s.ShiftCode == shiftCode &&
            (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(ShiftAssignment assignment, CancellationToken cancellationToken = default) =>
        _db.ShiftAssignments.AddAsync(assignment, cancellationToken).AsTask();
}
