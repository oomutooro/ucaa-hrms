using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UCAA.HRMS.Application.Services;

namespace UCAA.HRMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IJobArchitectureService, JobArchitectureService>();
        services.AddScoped<IRecruitmentService, RecruitmentService>();

        return services;
    }
}
