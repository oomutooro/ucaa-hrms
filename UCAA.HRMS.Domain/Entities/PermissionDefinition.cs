namespace UCAA.HRMS.Domain.Entities;

public static class PermissionDefinition
{
    public const string ManageEmployees = "employees.manage";
    public const string ManageDepartments = "departments.manage";
    public const string ManageLeave = "leave.manage";
    public const string ApproveLeave = "leave.approve";
    public const string ManagePayroll = "payroll.manage";
    public const string ManageShifts = "shifts.manage";
    public const string ManageDocuments = "documents.manage";

    public static IReadOnlyCollection<string> All =>
        new[]
        {
            ManageEmployees,
            ManageDepartments,
            ManageLeave,
            ApproveLeave,
            ManagePayroll,
            ManageShifts,
            ManageDocuments
        };
}
