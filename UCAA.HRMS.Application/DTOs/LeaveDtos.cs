using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.DTOs;

public sealed record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    LeaveType LeaveType,
    LeaveStatus Status,
    DateOnly StartDate,
    DateOnly EndDate,
    int RequestedDays,
    int? SickLeavePayPercent,
    string Reason,
    string? ReviewerComment);

public sealed record LeavePolicyRuleDto(
    LeaveType LeaveType,
    string LeaveTypeLabel,
    int MaxDaysPerRequest);

public sealed record LeaveBalanceDto(
    Guid EmployeeId,
    string EmployeeName,
    string DepartmentName,
    int AnnualLeaveEntitlementDays,
    decimal AnnualLeaveBalanceDays,
    decimal AnnualLeaveUsedDays);

public sealed record LeaveSummaryDto(
    int TotalRequests,
    int PendingRequests,
    int ApprovedRequests,
    int RejectedRequests,
    int EmployeesCurrentlyOnLeave,
    int UpcomingApprovedLeaves,
    List<LeavePolicyRuleDto> PolicyRules,
    List<LeaveBalanceDto> LeaveBalances);

public sealed record ApplyLeaveRequest(
    Guid EmployeeId,
    LeaveType LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason);

public sealed record ReviewLeaveRequest(LeaveStatus Status, string? ReviewerComment);
