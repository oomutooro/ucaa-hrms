using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Abstractions.Persistence;

public interface IDocumentRepository
{
    Task<List<HrDocument>> ListAsync(CancellationToken cancellationToken = default);
    Task<HrDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(HrDocument document, CancellationToken cancellationToken = default);
}
