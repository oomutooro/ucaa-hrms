using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Common;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Entities;

namespace UCAA.HRMS.Application.Services;

public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departments;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IDepartmentRepository departments, IUnitOfWork unitOfWork)
    {
        _departments = departments;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<DepartmentDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var data = await _departments.ListAsync(cancellationToken);
        return data.Select(d => new DepartmentDto(d.Id, d.Name, d.ParentDepartmentId)).ToList();
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _departments.NameExistsAsync(request.Name, cancellationToken: cancellationToken))
        {
            throw new AppException("Department name already exists.");
        }

        if (request.ParentDepartmentId.HasValue)
        {
            var parent = await _departments.GetByIdAsync(request.ParentDepartmentId.Value, cancellationToken);
            if (parent is null)
            {
                throw new AppException("Parent department not found.", 404);
            }
        }

        var department = new Department
        {
            Name = request.Name,
            ParentDepartmentId = request.ParentDepartmentId
        };

        await _departments.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DepartmentDto(department.Id, department.Name, department.ParentDepartmentId);
    }
}
