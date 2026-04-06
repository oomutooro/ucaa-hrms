using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Domain.Enums;
using UCAA.HRMS.Infrastructure.Auth;

namespace UCAA.HRMS.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        await db.Database.MigrateAsync();

        var roles = new[] { "Admin", "HR Manager", "Supervisor", "Employee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var adminEmail = "admin@ucaa.go.ug";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmployeeId = "UCAA-ADMIN-001",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "Admin@12345");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (!await db.Departments.AnyAsync())
        {
            var engineering = new Department { Name = "Engineering" };
            var electrical = new Department { Name = "Electrical", ParentDepartment = engineering };
            var civil = new Department { Name = "Civil", ParentDepartment = engineering };
            var electronics = new Department { Name = "Electronics", ParentDepartment = engineering };

            db.Departments.AddRange(engineering, electrical, civil, electronics);
            await db.SaveChangesAsync();

            db.Employees.Add(new Employee
            {
                FullName = "Jane Namugenyi",
                Email = "jane.namugenyi@ucaa.go.ug",
                PhoneNumber = "+256700000001",
                EmployeeId = "UCAA-EMP-001",
                DepartmentId = engineering.Id,
                JobTitle = "Senior Engineer",
                EmploymentType = EmploymentType.Permanent,
                AnnualLeaveBalanceDays = 30
            });

            await db.SaveChangesAsync();
        }
    }
}
