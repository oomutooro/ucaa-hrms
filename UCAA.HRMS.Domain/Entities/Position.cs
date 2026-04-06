using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

public sealed class Position : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    public Guid JobDescriptionId { get; set; }
    public JobDescription? JobDescription { get; set; }
    public int ApprovedHeadcount { get; set; } = 1;
}
