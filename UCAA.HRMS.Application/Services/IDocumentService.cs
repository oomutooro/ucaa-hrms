using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Services;

public interface IDocumentService
{
    Task<List<DocumentDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<DocumentDto> UploadAsync(CreateDocumentRequest request, Stream stream, string originalFileName, string contentType, CancellationToken cancellationToken = default);
}
