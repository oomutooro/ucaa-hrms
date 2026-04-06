using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

public sealed class JobDescription : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string PurposeStatement { get; set; } = string.Empty;
    public string KeyAccountabilities { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public Guid JobGradeId { get; set; }
    public JobGrade? JobGrade { get; set; }
    public ICollection<Position> Positions { get; set; } = new List<Position>();
}
