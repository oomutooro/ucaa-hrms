using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

public sealed class PayrollRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public DateOnly PayPeriod { get; set; }
    public string? Notes { get; set; }
}
