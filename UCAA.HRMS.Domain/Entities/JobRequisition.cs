using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class JobRequisition : BaseEntity
{
    public string RequisitionNumber { get; set; } = string.Empty;  // e.g. REQ-2026-0001
    public Guid PositionId { get; set; }
    public Position? Position { get; set; }
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    public int VacanciesRequested { get; set; } = 1;
    public string Justification { get; set; } = string.Empty;
    public DateOnly ClosingDate { get; set; }
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
