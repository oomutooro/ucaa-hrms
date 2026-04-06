using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IPayrollRepository
{
    Task<List<PayrollRecord>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(PayrollRecord payrollRecord, CancellationToken cancellationToken = default);
}
