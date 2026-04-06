using UCAA.HRMS.Domain.Common;

namespace UCAA.HRMS.Domain.Entities;

public sealed class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
