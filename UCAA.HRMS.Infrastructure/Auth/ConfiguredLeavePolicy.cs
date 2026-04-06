using Microsoft.Extensions.Options;
using UCAA.HRMS.Application.Services;
using UCAA.HRMS.Domain.Enums;
using UCAA.HRMS.Infrastructure.Options;

namespace UCAA.HRMS.Infrastructure.Auth;

public sealed class ConfiguredLeavePolicy : ILeavePolicy
{
    private readonly LeavePolicyOptions _options;

    public ConfiguredLeavePolicy(IOptions<LeavePolicyOptions> options)
    {
        _options = options.Value;
    }

    public int GetMaxDaysPerRequest(LeaveType leaveType) => leaveType switch
    {
        LeaveType.Annual => _options.AnnualMaxDaysPerRequest,
        LeaveType.Sick => _options.SickMaxDaysPerRequest,
        LeaveType.Maternity => _options.MaternityMaxDaysPerRequest,
        LeaveType.Paternity => _options.PaternityMaxDaysPerRequest,
        LeaveType.Compassionate => _options.CompassionateMaxDaysPerRequest,
        LeaveType.Study => _options.StudyMaxDaysPerRequest,
        LeaveType.Emergency => _options.EmergencyMaxDaysPerRequest,
        _ => 5
    };
}
