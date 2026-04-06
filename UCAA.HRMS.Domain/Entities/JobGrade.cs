using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

public sealed class JobGrade : BaseEntity
{
    public string GradeCode { get; set; } = string.Empty;   // e.g. "Grade 13"
    public string GradeTitle { get; set; } = string.Empty;  // e.g. "Executive Grade 1A"
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public ICollection<JobDescription> JobDescriptions { get; set; } = new List<JobDescription>();
}
