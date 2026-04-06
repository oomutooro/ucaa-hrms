namespace UCAA.HRMS.Infrastructure.Options;

public sealed class LeavePolicyOptions
{
    public const string SectionName = "LeavePolicy";

    public int AnnualMaxDaysPerRequest { get; set; } = 30;
    public int SickMaxDaysPerRequest { get; set; } = 14;
    public int EmergencyMaxDaysPerRequest { get; set; } = 5;
}
