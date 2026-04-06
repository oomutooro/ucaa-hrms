using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<List<Employee>> ListAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> EmployeeIdentifierExistsAsync(string employeeId, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    void Update(Employee employee);
    void Remove(Employee employee);
}
