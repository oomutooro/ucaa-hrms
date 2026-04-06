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

        async Task<Department> EnsureDepartmentAsync(string name, Guid? parentDepartmentId = null)
        {
            var existingDepartment = await db.Departments
                .FirstOrDefaultAsync(d => d.Name == name && d.ParentDepartmentId == parentDepartmentId);
            if (existingDepartment is not null)
            {
                return existingDepartment;
            }

            var department = new Department
            {
                Name = name,
                ParentDepartmentId = parentDepartmentId
            };

            db.Departments.Add(department);
            await db.SaveChangesAsync();

            return department;
        }

        // Seed UCAA organizational structure as Directorate -> Department hierarchy.
        var corporate = await EnsureDepartmentAsync("Corporate");
        var dhra = await EnsureDepartmentAsync("Directorate of Human Resources and Administration");
        var financeDirectorate = await EnsureDepartmentAsync("Directorate of Finance");
        var daas = await EnsureDepartmentAsync("Directorate of Airports and Aviation Security");
        var dsser = await EnsureDepartmentAsync("Directorate of Safety, Security and Economic Regulation");
        var dans = await EnsureDepartmentAsync("Directorate of Air Navigation Services");

        var shouldSeedOrgData = string.Equals(
            Environment.GetEnvironmentVariable("SEED_ORG_DATA"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (!shouldSeedOrgData)
        {
            return;
        }

        // Corporate departments
        var legal = await EnsureDepartmentAsync("Legal", corporate.Id);
        var publicAffairs = await EnsureDepartmentAsync("Public Affairs", corporate.Id);
        var informationTechnology = await EnsureDepartmentAsync("Information Technology", corporate.Id);
        var internalAudit = await EnsureDepartmentAsync("Internal Audit and Risk Management", corporate.Id);
        var procurement = await EnsureDepartmentAsync("Procurement", corporate.Id);
        var strategicPlanning = await EnsureDepartmentAsync("Strategic Planning", corporate.Id);
        var qualityAssurance = await EnsureDepartmentAsync("Quality Assurance", corporate.Id);
        var marketingCustomerService = await EnsureDepartmentAsync("Marketing and Customer Service", corporate.Id);
        var commercialServices = await EnsureDepartmentAsync("Commercial Services", corporate.Id);

        // Directorate of Human Resources and Administration departments
        var humanResources = await EnsureDepartmentAsync("Human Resources", dhra.Id);
        var hrTraining = await EnsureDepartmentAsync("Human Resource Development and Training", dhra.Id);
        var adminEstatesTransport = await EnsureDepartmentAsync("Administration, Estates and Transport", dhra.Id);

        // Directorate of Finance departments
        var finance = await EnsureDepartmentAsync("Finance", financeDirectorate.Id);
        var accounting = await EnsureDepartmentAsync("Accounting", financeDirectorate.Id);
        var managementAccounting = await EnsureDepartmentAsync("Management Accounting", financeDirectorate.Id);
        var supplies = await EnsureDepartmentAsync("Supplies", financeDirectorate.Id);

        // Directorate of Airports and Aviation Security departments
        var eia = await EnsureDepartmentAsync("Entebbe International Airport", daas.Id);
        var aviationSecurity = await EnsureDepartmentAsync("Aviation Security", daas.Id);
        var aerodromesMaintenance = await EnsureDepartmentAsync("Aerodromes Maintenance", daas.Id);
        var aerodromeEngineering = await EnsureDepartmentAsync("Aerodrome Engineering, Planning and Development", daas.Id);
        var eiaOperations = await EnsureDepartmentAsync("Operations - Entebbe International Airport", daas.Id);
        var regionalAirports = await EnsureDepartmentAsync("Regional Airports", daas.Id);
        var daasSmsQa = await EnsureDepartmentAsync("Safety Management Systems and Quality Assurance", daas.Id);
        var arffs = await EnsureDepartmentAsync("Aerodrome Rescue and Fire Fighting Services", daas.Id);
        var daasMarketingCommercial = await EnsureDepartmentAsync("Marketing and Commercial Services", daas.Id);

        // Directorate of Safety, Security and Economic Regulation departments
        var flightSafetyStandards = await EnsureDepartmentAsync("Flight Safety Standards", dsser.Id);
        var ansAerodromeStandards = await EnsureDepartmentAsync("Air Navigation Services and Aerodrome Standards", dsser.Id);
        var avsecFacilitationPolicy = await EnsureDepartmentAsync("Aviation Security and Facilitation Policy", dsser.Id);
        var economicRegulation = await EnsureDepartmentAsync("Economic Regulation", dsser.Id);

        // Directorate of Air Navigation Services departments
        var airTrafficManagement = await EnsureDepartmentAsync("Air Traffic Management", dans.Id);
        var communicationNavigationSurveillance = await EnsureDepartmentAsync("Communication Navigation Surveillance", dans.Id);
        var aimMetLiaison = await EnsureDepartmentAsync("Aeronautical Information Management and MET Liaison", dans.Id);
        var dansSmsQa = await EnsureDepartmentAsync("Safety Management Systems and Quality Assurance (DANS)", dans.Id);

        // Corporate sections
        await EnsureDepartmentAsync("Board Affairs and Compliance", legal.Id);
        await EnsureDepartmentAsync("Media and Stakeholder Relations", publicAffairs.Id);
        await EnsureDepartmentAsync("Database Administration", informationTechnology.Id);
        await EnsureDepartmentAsync("Network Administration", informationTechnology.Id);
        await EnsureDepartmentAsync("Systems Administration", informationTechnology.Id);
        await EnsureDepartmentAsync("Systems Support", informationTechnology.Id);
        await EnsureDepartmentAsync("Information Security", informationTechnology.Id);
        await EnsureDepartmentAsync("ICT Innovation", informationTechnology.Id);
        await EnsureDepartmentAsync("IT Support and Help Desk", informationTechnology.Id);
        await EnsureDepartmentAsync("Audit Services", internalAudit.Id);
        await EnsureDepartmentAsync("Risk Management", internalAudit.Id);
        await EnsureDepartmentAsync("Compliance Audit", internalAudit.Id);
        await EnsureDepartmentAsync("ICT Audit", internalAudit.Id);
        await EnsureDepartmentAsync("Procurement Operations", procurement.Id);
        await EnsureDepartmentAsync("Procurement Registry", procurement.Id);
        await EnsureDepartmentAsync("Policy, Planning and Budgeting", strategicPlanning.Id);
        await EnsureDepartmentAsync("Statistics and Data Analytics", strategicPlanning.Id);
        await EnsureDepartmentAsync("Quality Management", qualityAssurance.Id);
        await EnsureDepartmentAsync("Customer Service", marketingCustomerService.Id);
        await EnsureDepartmentAsync("Call Centre", marketingCustomerService.Id);
        await EnsureDepartmentAsync("Research", marketingCustomerService.Id);
        await EnsureDepartmentAsync("Commercial Operations", commercialServices.Id);

        // HR and Administration sections
        await EnsureDepartmentAsync("HR Business Partnering", humanResources.Id);
        await EnsureDepartmentAsync("Compensation and Benefits", humanResources.Id);
        await EnsureDepartmentAsync("Performance and Talent Management", humanResources.Id);
        await EnsureDepartmentAsync("Training and Development", hrTraining.Id);
        await EnsureDepartmentAsync("Administration and Estates", adminEstatesTransport.Id);
        await EnsureDepartmentAsync("Registry and Digitized Systems", adminEstatesTransport.Id);
        await EnsureDepartmentAsync("Transport and Maintenance", adminEstatesTransport.Id);

        // Finance sections
        await EnsureDepartmentAsync("Revenue Billing", accounting.Id);
        await EnsureDepartmentAsync("Debt Management", accounting.Id);
        await EnsureDepartmentAsync("Revenue Collection", accounting.Id);
        await EnsureDepartmentAsync("Expenditure", accounting.Id);
        await EnsureDepartmentAsync("Tax and Asset Management", accounting.Id);
        await EnsureDepartmentAsync("Regional Airports Accounting", accounting.Id);
        await EnsureDepartmentAsync("Pay and Benefits", finance.Id);
        await EnsureDepartmentAsync("Management Reporting", managementAccounting.Id);
        await EnsureDepartmentAsync("Financial Planning and Analysis", managementAccounting.Id);
        await EnsureDepartmentAsync("Stores and Inventory", supplies.Id);
        await EnsureDepartmentAsync("Verification", finance.Id);

        // DAAS sections
        await EnsureDepartmentAsync("Airport Operations", eia.Id);
        await EnsureDepartmentAsync("VVIP and VIP Services", eia.Id);
        await EnsureDepartmentAsync("Security Operations", aviationSecurity.Id);
        await EnsureDepartmentAsync("Civil Maintenance", aerodromesMaintenance.Id);
        await EnsureDepartmentAsync("Electrical and Electronics Maintenance", aerodromesMaintenance.Id);
        await EnsureDepartmentAsync("Mechanical Maintenance", aerodromesMaintenance.Id);
        await EnsureDepartmentAsync("Aerodrome Planning and Development", aerodromeEngineering.Id);
        await EnsureDepartmentAsync("Terminal Operations", eiaOperations.Id);
        await EnsureDepartmentAsync("Airside Operations", eiaOperations.Id);
        await EnsureDepartmentAsync("Hoima Airport", regionalAirports.Id);
        await EnsureDepartmentAsync("Gulu Airport", regionalAirports.Id);
        await EnsureDepartmentAsync("Arua Airport", regionalAirports.Id);
        await EnsureDepartmentAsync("Soroti Airport", regionalAirports.Id);
        await EnsureDepartmentAsync("Safety Assurance", daasSmsQa.Id);
        await EnsureDepartmentAsync("Fire and Rescue Operations", arffs.Id);
        await EnsureDepartmentAsync("Airport Commercial Services", daasMarketingCommercial.Id);

        // DSSER sections
        await EnsureDepartmentAsync("Airworthiness", flightSafetyStandards.Id);
        await EnsureDepartmentAsync("Flight Operations", flightSafetyStandards.Id);
        await EnsureDepartmentAsync("Personnel Licensing", flightSafetyStandards.Id);
        await EnsureDepartmentAsync("ANS Standards", ansAerodromeStandards.Id);
        await EnsureDepartmentAsync("Aerodrome Standards", ansAerodromeStandards.Id);
        await EnsureDepartmentAsync("AVSEC Policy", avsecFacilitationPolicy.Id);
        await EnsureDepartmentAsync("Facilitation", avsecFacilitationPolicy.Id);
        await EnsureDepartmentAsync("Tariffs and Charges", economicRegulation.Id);
        await EnsureDepartmentAsync("Consumer Protection", economicRegulation.Id);

        // DANS sections
        await EnsureDepartmentAsync("Area Control Centre", airTrafficManagement.Id);
        await EnsureDepartmentAsync("Tower and Approach Services", airTrafficManagement.Id);
        await EnsureDepartmentAsync("Communications", communicationNavigationSurveillance.Id);
        await EnsureDepartmentAsync("Navigation", communicationNavigationSurveillance.Id);
        await EnsureDepartmentAsync("Surveillance", communicationNavigationSurveillance.Id);
        await EnsureDepartmentAsync("Aeronautical Information Services", aimMetLiaison.Id);
        await EnsureDepartmentAsync("Meteorology Liaison", aimMetLiaison.Id);
        await EnsureDepartmentAsync("NOTAM and Briefing", aimMetLiaison.Id);
        await EnsureDepartmentAsync("Safety and Compliance", dansSmsQa.Id);

        var seedEmployee = await db.Employees.FirstOrDefaultAsync(e => e.EmployeeId == "UCAA-EMP-001");
        if (seedEmployee is null)
        {
            db.Employees.Add(new Employee
            {
                FullName = "Jane Namugenyi",
                Email = "jane.namugenyi@ucaa.go.ug",
                PhoneNumber = "+256700000001",
                EmployeeId = "UCAA-EMP-001",
                DateOfBirth = new DateOnly(1990, 1, 1),
                FirstEmploymentDate = new DateOnly(2015, 1, 1),
                JobLevel = 10,
                DepartmentId = dhra.Id,
                JobTitle = "Senior Human Resource Officer",
                EmploymentType = EmploymentType.Permanent,
                AnnualLeaveBalanceDays = 36
            });
        }
        else
        {
            seedEmployee.DepartmentId = dhra.Id;
            seedEmployee.JobTitle = "Senior Human Resource Officer";
            seedEmployee.DateOfBirth = seedEmployee.DateOfBirth == default ? new DateOnly(1990, 1, 1) : seedEmployee.DateOfBirth;
            seedEmployee.FirstEmploymentDate = seedEmployee.FirstEmploymentDate == default ? new DateOnly(2015, 1, 1) : seedEmployee.FirstEmploymentDate;
            seedEmployee.JobLevel = seedEmployee.JobLevel <= 0 ? 10 : seedEmployee.JobLevel;
            seedEmployee.Email ??= "jane.namugenyi@ucaa.go.ug";
            seedEmployee.PhoneNumber ??= "+256700000001";
        }

        // Remove old placeholder hierarchy from early bootstrap seeds when safe.
        var legacyRoot = await db.Departments.FirstOrDefaultAsync(d => d.Name == "Engineering" && d.ParentDepartmentId == null);
        if (legacyRoot is not null)
        {
            var legacyIds = await db.Departments
                .Where(d => d.Id == legacyRoot.Id || d.ParentDepartmentId == legacyRoot.Id)
                .Select(d => d.Id)
                .ToListAsync();

            var inUse = await db.Employees.AnyAsync(e => legacyIds.Contains(e.DepartmentId));
            if (!inUse)
            {
                var legacyChildren = await db.Departments.Where(d => d.ParentDepartmentId == legacyRoot.Id).ToListAsync();
                db.Departments.RemoveRange(legacyChildren);
                db.Departments.Remove(legacyRoot);
            }
        }

        await db.SaveChangesAsync();
    }
}
