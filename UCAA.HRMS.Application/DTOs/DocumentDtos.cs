using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record DocumentDto(
    Guid Id,
    Guid? EmployeeId,
    DocumentType DocumentType,
    string FileName,
    string ContentType,
    long FileSizeBytes);

public sealed record CreateDocumentRequest(Guid? EmployeeId, DocumentType DocumentType);
