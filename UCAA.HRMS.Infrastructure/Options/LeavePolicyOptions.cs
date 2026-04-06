namespace UCAA.HRMS.Infrastructure.Options;

public sealed class LeavePolicyOptions
{
    public const string SectionName = "LeavePolicy";

    public int AnnualMaxDaysPerRequest { get; set; } = 36;
    public int SickMaxDaysPerRequest { get; set; } = 14;
    public int MaternityMaxDaysPerRequest { get; set; } = 60;
    public int PaternityMaxDaysPerRequest { get; set; } = 4;
    public int CompassionateMaxDaysPerRequest { get; set; } = 5;
    public int StudyMaxDaysPerRequest { get; set; } = 180;
    public int EmergencyMaxDaysPerRequest { get; set; } = 5;
}
