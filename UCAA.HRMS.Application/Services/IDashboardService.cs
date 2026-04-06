using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IDashboardService
{
    Task<DashboardMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default);
}
