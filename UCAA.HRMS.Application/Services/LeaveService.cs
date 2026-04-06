using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Services;

public sealed class LeaveService : ILeaveService
{
    private readonly ILeaveRequestRepository _leaveRequests;
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeavePolicy _leavePolicy;

    public LeaveService(
        ILeaveRequestRepository leaveRequests,
        IEmployeeRepository employees,
        IUnitOfWork unitOfWork,
        ILeavePolicy leavePolicy)
    {
        _leaveRequests = leaveRequests;
        _employees = employees;
        _unitOfWork = unitOfWork;
        _leavePolicy = leavePolicy;
    }

    public async Task<List<LeaveRequestDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var data = await _leaveRequests.ListAsync(cancellationToken);
        return data.Select(Map).ToList();
    }

    public async Task<LeaveRequestDto> ApplyAsync(ApplyLeaveRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EndDate < request.StartDate)
        {
            throw new AppException("End date cannot be before start date.");
        }

        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new AppException("Employee not found.", 404);

        var requestedDays = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;
        if (requestedDays > _leavePolicy.GetMaxDaysPerRequest(request.LeaveType))
        {
            throw new AppException("Requested leave days exceed configured limit.");
        }

        if (request.LeaveType == LeaveType.Compassionate)
        {
            var periodStart = request.StartDate.AddDays(-365);
            var employeeLeaveHistory = await _leaveRequests.ListAsync(cancellationToken);
            var compassionateDaysInLastYear = employeeLeaveHistory
                .Where(x => x.EmployeeId == request.EmployeeId
                            && x.LeaveType == LeaveType.Compassionate
                            && x.Status != LeaveStatus.Rejected
                            && x.StartDate >= periodStart
                            && x.StartDate <= request.StartDate)
                .Sum(x => x.EndDate.DayNumber - x.StartDate.DayNumber + 1);

            if (compassionateDaysInLastYear + requestedDays > 14)
            {
                throw new AppException("Compassionate leave cannot exceed 14 days within 12 months.");
            }
        }

        if (request.LeaveType == LeaveType.Annual && employee.AnnualLeaveBalanceDays < requestedDays)
        {
            throw new AppException("Insufficient annual leave balance.");
        }

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = request.EmployeeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason
        };

        await _leaveRequests.AddAsync(leaveRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        leaveRequest.Employee = employee;
        return Map(leaveRequest);
    }

    public async Task<LeaveRequestDto> ReviewAsync(Guid id, ReviewLeaveRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Status == LeaveStatus.Pending)
        {
            throw new AppException("Review decision must be Approved or Rejected.");
        }

        var leaveRequest = await _leaveRequests.GetByIdAsync(id, cancellationToken)
            ?? throw new AppException("Leave request not found.", 404);

        leaveRequest.Status = request.Status;
        leaveRequest.ReviewerComment = request.ReviewerComment;
        leaveRequest.UpdatedAtUtc = DateTime.UtcNow;

        if (leaveRequest.Status == LeaveStatus.Approved && leaveRequest.LeaveType == LeaveType.Annual)
        {
            var employee = await _employees.GetByIdAsync(leaveRequest.EmployeeId, cancellationToken)
                ?? throw new AppException("Employee not found.", 404);
            var approvedDays = leaveRequest.EndDate.DayNumber - leaveRequest.StartDate.DayNumber + 1;
            employee.AnnualLeaveBalanceDays -= approvedDays;
            _employees.Update(employee);
        }

        _leaveRequests.Update(leaveRequest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(leaveRequest);
    }

    private static LeaveRequestDto Map(LeaveRequest leaveRequest) =>
        new(
            leaveRequest.Id,
            leaveRequest.EmployeeId,
            leaveRequest.Employee?.FullName ?? string.Empty,
            leaveRequest.LeaveType,
            leaveRequest.Status,
            leaveRequest.StartDate,
            leaveRequest.EndDate,
            leaveRequest.EndDate.DayNumber - leaveRequest.StartDate.DayNumber + 1,
            GetSickLeavePayPercent(leaveRequest),
            leaveRequest.Reason,
            leaveRequest.ReviewerComment);

    private static int? GetSickLeavePayPercent(LeaveRequest leaveRequest)
    {
        if (leaveRequest.LeaveType != LeaveType.Sick)
        {
            return null;
        }

        var requestedDays = leaveRequest.EndDate.DayNumber - leaveRequest.StartDate.DayNumber + 1;
        if (requestedDays <= 180)
        {
            return 100;
        }

        if (requestedDays <= 270)
        {
            return 75;
        }

        return 50;
    }
}
