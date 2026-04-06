using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public interface ILeavePolicy
{
    int GetMaxDaysPerRequest(LeaveType leaveType);
}
