using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _db;

    public EmployeeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<Employee>> ListAsync(CancellationToken cancellationToken = default) =>
        _db.Employees.Include(e => e.Department).OrderBy(e => e.FullName).ToListAsync(cancellationToken);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        _db.Employees.AnyAsync(e => e.Email.ToLower() == email.ToLower() && (!excludeId.HasValue || e.Id != excludeId.Value), cancellationToken);

    public Task<bool> EmployeeIdentifierExistsAsync(string employeeId, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        _db.Employees.AnyAsync(e => e.EmployeeId.ToLower() == employeeId.ToLower() && (!excludeId.HasValue || e.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(Employee employee, CancellationToken cancellationToken = default) => _db.Employees.AddAsync(employee, cancellationToken).AsTask();

    public void Update(Employee employee) => _db.Employees.Update(employee);

    public void Remove(Employee employee) => _db.Employees.Remove(employee);
}
