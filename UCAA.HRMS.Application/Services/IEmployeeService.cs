using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<EmployeeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
