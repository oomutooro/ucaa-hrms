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
    public DbSet<JobGrade> JobGrades => Set<JobGrade>();
    public DbSet<JobDescription> JobDescriptions => Set<JobDescription>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<JobRequisition> JobRequisitions => Set<JobRequisition>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<OnboardingTemplate> OnboardingTemplates => Set<OnboardingTemplate>();
    public DbSet<OnboardingTemplateTask> OnboardingTemplateTasks => Set<OnboardingTemplateTask>();
    public DbSet<EmployeeOnboarding> EmployeeOnboardings => Set<EmployeeOnboarding>();
    public DbSet<OnboardingItem> OnboardingItems => Set<OnboardingItem>();

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

        builder.Entity<JobGrade>(cfg =>
        {
            cfg.HasIndex(g => g.GradeCode).IsUnique();
            cfg.Property(g => g.GradeCode).HasMaxLength(40).IsRequired();
            cfg.Property(g => g.GradeTitle).HasMaxLength(100).IsRequired();
            cfg.Property(g => g.MinSalary).HasColumnType("decimal(18,2)").IsRequired();
            cfg.Property(g => g.MaxSalary).HasColumnType("decimal(18,2)").IsRequired();
        });

        builder.Entity<JobDescription>(cfg =>
        {
            cfg.Property(j => j.Title).HasMaxLength(120).IsRequired();
            cfg.Property(j => j.PurposeStatement).HasMaxLength(1000).IsRequired();
            cfg.Property(j => j.KeyAccountabilities).HasMaxLength(4000).IsRequired();
            cfg.Property(j => j.Qualifications).HasMaxLength(2000).IsRequired();
            cfg.HasOne(j => j.JobGrade)
                .WithMany(g => g.JobDescriptions)
                .HasForeignKey(j => j.JobGradeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Position>(cfg =>
        {
            cfg.Property(p => p.Title).HasMaxLength(120).IsRequired();
            cfg.HasOne(p => p.Department)
                .WithMany()
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            cfg.HasOne(p => p.JobDescription)
                .WithMany(j => j.Positions)
                .HasForeignKey(p => p.JobDescriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<JobRequisition>(cfg =>
        {
            cfg.HasIndex(r => r.RequisitionNumber).IsUnique();
            cfg.Property(r => r.RequisitionNumber).HasMaxLength(30).IsRequired();
            cfg.Property(r => r.Justification).HasMaxLength(1000).IsRequired();
            cfg.HasOne(r => r.Position)
                .WithMany()
                .HasForeignKey(r => r.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
            cfg.HasOne(r => r.Department)
                .WithMany()
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<JobApplication>(cfg =>
        {
            cfg.Property(a => a.ApplicantName).HasMaxLength(120).IsRequired();
            cfg.Property(a => a.ApplicantEmail).HasMaxLength(120).IsRequired();
            cfg.Property(a => a.ApplicantPhone).HasMaxLength(20).IsRequired();
            cfg.Property(a => a.ReviewNotes).HasMaxLength(1000);
            cfg.HasOne(a => a.Requisition)
                .WithMany(r => r.Applications)
                .HasForeignKey(a => a.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OnboardingTemplate>(cfg =>
        {
            cfg.Property(t => t.Name).HasMaxLength(120).IsRequired();
            cfg.Property(t => t.Description).HasMaxLength(500).IsRequired();
        });

        builder.Entity<OnboardingTemplateTask>(cfg =>
        {
            cfg.Property(t => t.Title).HasMaxLength(200).IsRequired();
            cfg.Property(t => t.Category).HasMaxLength(60).IsRequired();
            cfg.HasOne(t => t.Template)
                .WithMany(tp => tp.Tasks)
                .HasForeignKey(t => t.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<EmployeeOnboarding>(cfg =>
        {
            cfg.Property(ob => ob.Notes).HasMaxLength(1000);
            cfg.HasOne(ob => ob.Employee)
                .WithMany()
                .HasForeignKey(ob => ob.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OnboardingItem>(cfg =>
        {
            cfg.Property(i => i.Title).HasMaxLength(200).IsRequired();
            cfg.Property(i => i.Category).HasMaxLength(60).IsRequired();
            cfg.Property(i => i.Notes).HasMaxLength(1000);
            cfg.HasOne(i => i.Onboarding)
                .WithMany(ob => ob.Items)
                .HasForeignKey(i => i.OnboardingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
