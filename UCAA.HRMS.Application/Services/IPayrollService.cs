using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IPayrollService
{
    Task<List<PayrollRecordDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<PayrollRecordDto> CreateAsync(CreatePayrollRecordRequest request, CancellationToken cancellationToken = default);
}
