using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);
}
