using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet]
    public Task<List<DocumentDto>> GetAll(CancellationToken cancellationToken) =>
        _documentService.ListAsync(cancellationToken);

    [HttpPost("upload")]
    [Authorize(Roles = "Admin,HR Manager")]
    [RequestSizeLimit(20_000_000)]
    public async Task<DocumentDto> Upload(
        [FromForm] IFormFile file,
        [FromForm] Guid? employeeId,
        [FromForm] DocumentType documentType,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var request = new CreateDocumentRequest(employeeId, documentType);
        return await _documentService.UploadAsync(request, stream, file.FileName, file.ContentType, cancellationToken);
    }
}
