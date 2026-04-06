using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Abstractions.Storage;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Services;

public sealed class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documents;
    private readonly IFileStorageService _storage;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentService(IDocumentRepository documents, IFileStorageService storage, IUnitOfWork unitOfWork)
    {
        _documents = documents;
        _storage = storage;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<DocumentDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _documents.ListAsync(cancellationToken);
        return items.Select(d => new DocumentDto(d.Id, d.EmployeeId, d.DocumentType, d.FileName, d.ContentType, d.FileSizeBytes)).ToList();
    }

    public async Task<DocumentDto> UploadAsync(CreateDocumentRequest request, Stream stream, string originalFileName, string contentType, CancellationToken cancellationToken = default)
    {
        var stored = await _storage.SaveAsync(stream, originalFileName, cancellationToken);

        var document = new HrDocument
        {
            EmployeeId = request.EmployeeId,
            DocumentType = request.DocumentType,
            FileName = originalFileName,
            StoredFileName = stored,
            ContentType = contentType,
            FileSizeBytes = stream.CanSeek ? stream.Length : 0
        };

        await _documents.AddAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DocumentDto(document.Id, document.EmployeeId, document.DocumentType, document.FileName, document.ContentType, document.FileSizeBytes);
    }
}
