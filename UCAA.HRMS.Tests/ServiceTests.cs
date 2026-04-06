using Moq;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Tests;

public sealed class ServiceTests
{
    [Fact]
    public async Task LeaveApproval_DeductsAnnualBalance()
    {
        var leaveRepo = new Mock<ILeaveRequestRepository>();
        var employeeRepo = new Mock<IEmployeeRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var leavePolicy = new Mock<ILeavePolicy>();

        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            FullName = "Test Employee",
            AnnualLeaveBalanceDays = 30
        };

        var leaveRequest = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveType = LeaveType.Annual,
            StartDate = new DateOnly(2026, 4, 1),
            EndDate = new DateOnly(2026, 4, 3)
        };

        leaveRepo.Setup(x => x.GetByIdAsync(leaveRequest.Id, It.IsAny<CancellationToken>())).ReturnsAsync(leaveRequest);
        employeeRepo.Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);

        var service = new LeaveService(leaveRepo.Object, employeeRepo.Object, unitOfWork.Object, leavePolicy.Object);

        await service.ReviewAsync(leaveRequest.Id, new ReviewLeaveRequest(LeaveStatus.Approved, "Approved"));

        Assert.Equal(27, employee.AnnualLeaveBalanceDays);
        employeeRepo.Verify(x => x.Update(employee), Times.Once);
    }

    [Fact]
    public async Task GenerateRotation_CreatesDayNightOffOffPattern()
    {
        var shiftRepo = new Mock<IShiftRepository>();
        var employeeRepo = new Mock<IEmployeeRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        shiftRepo.Setup(x => x.ExistsForDateAndShiftAsync(It.IsAny<DateOnly>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var employeeId = Guid.NewGuid();
        employeeRepo.Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Employee { Id = employeeId, FullName = "Shift Worker" });

        var service = new ShiftService(shiftRepo.Object, employeeRepo.Object, unitOfWork.Object);
        var employeeIds = new List<Guid> { employeeId };

        var result = await service.GenerateRotationAsync(new DateOnly(2026, 4, 1), 4, employeeIds);

        Assert.Equal(4, result.Count);
        Assert.Equal(ShiftType.Day, result[0].ShiftType);
        Assert.Equal(ShiftType.Night, result[1].ShiftType);
        Assert.Equal(ShiftType.Off, result[2].ShiftType);
        Assert.Equal(ShiftType.Off, result[3].ShiftType);
    }
}
