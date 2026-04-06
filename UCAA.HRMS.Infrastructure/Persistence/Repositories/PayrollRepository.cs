using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class PayrollRepository : IPayrollRepository
{
    private readonly ApplicationDbContext _db;

    public PayrollRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<PayrollRecord>> ListAsync(CancellationToken cancellationToken = default) =>
        _db.PayrollRecords.Include(p => p.Employee).OrderByDescending(p => p.PayPeriod).ToListAsync(cancellationToken);

    public Task<PayrollRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.PayrollRecords.Include(p => p.Employee).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExistsForEmployeeAndPeriodAsync(Guid employeeId, DateOnly payPeriod, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        _db.PayrollRecords.AnyAsync(p =>
            p.EmployeeId == employeeId &&
            p.PayPeriod == payPeriod &&
            (!excludeId.HasValue || p.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(PayrollRecord payrollRecord, CancellationToken cancellationToken = default) =>
        _db.PayrollRecords.AddAsync(payrollRecord, cancellationToken).AsTask();

    public void Update(PayrollRecord payrollRecord) => _db.PayrollRecords.Update(payrollRecord);
}
