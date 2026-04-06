using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Data;

namespace UCAA.HRMS.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _db;

    public DocumentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<HrDocument>> ListAsync(CancellationToken cancellationToken = default) =>
        _db.HrDocuments.OrderByDescending(d => d.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<HrDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.HrDocuments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public Task AddAsync(HrDocument document, CancellationToken cancellationToken = default) =>
        _db.HrDocuments.AddAsync(document, cancellationToken).AsTask();
}
