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

    public Task AddAsync(PayrollRecord payrollRecord, CancellationToken cancellationToken = default) =>
        _db.PayrollRecords.AddAsync(payrollRecord, cancellationToken).AsTask();
}
