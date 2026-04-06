using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IDepartmentRepository
{
    Task<List<Department>> ListAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Department department, CancellationToken cancellationToken = default);
    void Update(Department department);
    void Remove(Department department);
}
