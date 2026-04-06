using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IPayrollRepository
{
    Task<List<PayrollRecord>> ListAsync(CancellationToken cancellationToken = default);
    Task<PayrollRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsForEmployeeAndPeriodAsync(Guid employeeId, DateOnly payPeriod, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(PayrollRecord payrollRecord, CancellationToken cancellationToken = default);
    void Update(PayrollRecord payrollRecord);
}
