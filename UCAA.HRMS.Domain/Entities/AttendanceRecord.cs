using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class AttendanceRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public Guid? ShiftAssignmentId { get; set; }
    public ShiftAssignment? ShiftAssignment { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public TimeOnly CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    public decimal HoursWorked { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Notes { get; set; }
}