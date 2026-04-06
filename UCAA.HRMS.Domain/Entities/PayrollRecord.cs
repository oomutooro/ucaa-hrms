using UCAA.HRMS.Domain.Common;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Domain.Entities;

public sealed class PayrollRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal OtherAllowance { get; set; }
    public decimal Allowances { get; set; }
    public decimal PayeTax { get; set; }
    public decimal PensionDeduction { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal OtherDeduction { get; set; }
    public decimal Deductions { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public DateOnly PayPeriod { get; set; }
    public PayrollStatus Status { get; set; } = PayrollStatus.Draft;
    public DateTime? PaidAtUtc { get; set; }
    public string? Notes { get; set; }
}
