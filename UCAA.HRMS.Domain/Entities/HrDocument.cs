using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class HrDocument : BaseEntity
{
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}
