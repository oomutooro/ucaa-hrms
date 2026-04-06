using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public Task<List<DepartmentDto>> GetAll(CancellationToken cancellationToken) =>
        _departmentService.ListAsync(cancellationToken);

    [HttpPost]
    [Authorize(Roles = "Admin,HR Manager")]
    public Task<DepartmentDto> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken) =>
        _departmentService.CreateAsync(request, cancellationToken);
}
