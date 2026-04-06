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

        if (!request.ParentDepartmentId.HasValue)
        {
            throw new AppException("Select an existing directorate or department as the parent.");
        }

        if (request.ParentDepartmentId.HasValue)
        {
            var parent = await _departments.GetByIdAsync(request.ParentDepartmentId.Value, cancellationToken);
            if (parent is null)
            {
                throw new AppException("Parent department not found.", 404);
            }

            // Maximum supported hierarchy depth is 3: Directorate -> Department -> Section.
            if (parent.ParentDepartmentId.HasValue)
            {
                var grandParent = await _departments.GetByIdAsync(parent.ParentDepartmentId.Value, cancellationToken);
                if (grandParent?.ParentDepartmentId is not null)
                {
                    throw new AppException("Sections cannot have child sections. Select a directorate or department as parent.");
                }
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
