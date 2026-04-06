using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class Employee : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public Guid? UserId { get; set; }
    public decimal AnnualLeaveBalanceDays { get; set; } = 30;
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
