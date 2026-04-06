using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR Manager")]
public sealed class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpGet]
    public Task<List<PayrollRecordDto>> GetAll(CancellationToken cancellationToken) =>
        _payrollService.ListAsync(cancellationToken);

    [HttpGet("summary")]
    public Task<PayrollSummaryDto> GetSummary(CancellationToken cancellationToken) =>
        _payrollService.GetSummaryAsync(cancellationToken);

    [HttpPost]
    public Task<PayrollRecordDto> Create([FromBody] CreatePayrollRecordRequest request, CancellationToken cancellationToken) =>
        _payrollService.CreateAsync(request, cancellationToken);

    [HttpPatch("{id:guid}/status")]
    public Task<PayrollRecordDto> UpdateStatus(Guid id, [FromBody] UpdatePayrollStatusRequest request, CancellationToken cancellationToken) =>
        _payrollService.UpdateStatusAsync(id, request, cancellationToken);
}
