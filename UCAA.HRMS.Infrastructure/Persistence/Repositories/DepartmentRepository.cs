using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _db;

    public DepartmentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<Department>> ListAsync(CancellationToken cancellationToken = default) =>
        _db.Departments.OrderBy(d => d.Name).ToListAsync(cancellationToken);

    public Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        _db.Departments.AnyAsync(d => d.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || d.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(Department department, CancellationToken cancellationToken = default) => _db.Departments.AddAsync(department, cancellationToken).AsTask();

    public void Update(Department department) => _db.Departments.Update(department);

    public void Remove(Department department) => _db.Departments.Remove(department);
}
