using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UCAA.HRMS.Application.Abstractions.Auth;
using UCAA.HRMS.Application.Abstractions.Persistence;
using UCAA.HRMS.Application.Abstractions.Storage;
using UCAA.HRMS.Application.Services;
using UCAA.HRMS.Infrastructure.Auth;
using UCAA.HRMS.Infrastructure.Data;
using UCAA.HRMS.Infrastructure.Options;
using UCAA.HRMS.Infrastructure.Persistence.Repositories;
using UCAA.HRMS.Infrastructure.Storage;

namespace UCAA.HRMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<LeavePolicyOptions>(configuration.GetSection(LeavePolicyOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();

        services.AddScoped<IAuthProvider, LocalAuthProvider>();
        services.AddScoped<IMicrosoftAuthProvider, MicrosoftAuthProvider>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<ILeavePolicy, ConfiguredLeavePolicy>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IJobArchitectureRepository, JobArchitectureRepository>();
        services.AddScoped<IRecruitmentRepository, RecruitmentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var storageRoot = configuration["Storage:RootPath"] ?? Path.Combine(AppContext.BaseDirectory, "uploads");
        services.AddSingleton<IFileStorageService>(_ => new LocalFileStorageService(storageRoot));

        return services;
    }
}
