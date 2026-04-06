namespace UCAA.HRMS.Application.DTOs;

public sealed record DepartmentDto(Guid Id, string Name, Guid? ParentDepartmentId);

public sealed record CreateDepartmentRequest(string Name, Guid? ParentDepartmentId);
