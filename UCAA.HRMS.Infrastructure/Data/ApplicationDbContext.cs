using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UCAA.HRMS.Domain.Entities;
using UCAA.HRMS.Infrastructure.Auth;

namespace UCAA.HRMS.Infrastructure.Data;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<ShiftAssignment> ShiftAssignments => Set<ShiftAssignment>();
    public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();
    public DbSet<HrDocument> HrDocuments => Set<HrDocument>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>(cfg =>
        {
            cfg.HasIndex(e => e.Email).IsUnique();
            cfg.HasIndex(e => e.EmployeeId).IsUnique();
            cfg.Property(e => e.FullName).HasMaxLength(120).IsRequired();
            cfg.Property(e => e.Email).HasMaxLength(120).IsRequired();
            cfg.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
            cfg.Property(e => e.EmployeeId).HasMaxLength(40).IsRequired();
            cfg.Property(e => e.JobTitle).HasMaxLength(80).IsRequired();
            cfg.Property(e => e.JobLevel).IsRequired();
            cfg.Property(e => e.DateOfBirth).IsRequired();
            cfg.Property(e => e.FirstEmploymentDate).IsRequired();
            cfg.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Department>(cfg =>
        {
            cfg.HasIndex(d => d.Name).IsUnique();
            cfg.Property(d => d.Name).HasMaxLength(120).IsRequired();
            cfg.HasOne(d => d.ParentDepartment)
                .WithMany(d => d.ChildDepartments)
                .HasForeignKey(d => d.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<LeaveRequest>(cfg =>
        {
            cfg.Property(l => l.Reason).HasMaxLength(500).IsRequired();
            cfg.HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ShiftAssignment>(cfg =>
        {
            cfg.HasIndex(s => new { s.ShiftDate, s.ShiftCode }).IsUnique();
            cfg.Property(s => s.ShiftCode).HasMaxLength(40).IsRequired();
            cfg.HasOne(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<PayrollRecord>(cfg =>
        {
            cfg.HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<HrDocument>(cfg =>
        {
            cfg.Property(d => d.FileName).HasMaxLength(255).IsRequired();
            cfg.Property(d => d.StoredFileName).HasMaxLength(255).IsRequired();
            cfg.Property(d => d.ContentType).HasMaxLength(120).IsRequired();
            cfg.HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
