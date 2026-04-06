using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class LeaveRequest : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public LeaveType LeaveType { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReviewerComment { get; set; }
    public Guid? ReviewedByUserId { get; set; }
}
