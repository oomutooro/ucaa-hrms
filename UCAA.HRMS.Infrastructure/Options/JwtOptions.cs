namespace UCAA.HRMS.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "UCAA.HRMS";
    public string Audience { get; set; } = "UCAA.HRMS.Client";
    public string Key { get; set; } = "REPLACE_WITH_32_PLUS_CHAR_SECURE_KEY_FOR_PRODUCTION";
    public int ExpiryMinutes { get; set; } = 120;
}
