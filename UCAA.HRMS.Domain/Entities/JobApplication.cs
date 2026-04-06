using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class JobApplication : BaseEntity
{
    public Guid RequisitionId { get; set; }
    public JobRequisition? Requisition { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public bool IsInternal { get; set; }          // internal (existing employee) vs external
    public Guid? EmployeeId { get; set; }         // set when IsInternal = true
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Received;
    public string? ReviewNotes { get; set; }
    public DateOnly? InterviewDate { get; set; }
}
