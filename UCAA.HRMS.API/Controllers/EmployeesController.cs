using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public Task<List<EmployeeDto>> GetAll(CancellationToken cancellationToken) =>
        _employeeService.ListAsync(cancellationToken);

    [HttpPost]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<EmployeeDto> Create([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken) =>
        _employeeService.CreateAsync(request, cancellationToken);

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<EmployeeDto> Update(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken) =>
        _employeeService.UpdateAsync(id, request, cancellationToken);

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
