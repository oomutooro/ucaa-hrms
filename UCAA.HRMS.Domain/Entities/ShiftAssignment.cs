using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class ShiftAssignment : BaseEntity
{
    public DateOnly ShiftDate { get; set; }
    public ShiftType ShiftType { get; set; }
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public string ShiftCode { get; set; } = string.Empty;
}
