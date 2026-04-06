using FluentValidation;
using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Validation;

public sealed class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.ParentDepartmentId)
            .NotNull()
            .WithMessage("Select an existing directorate or department as parent.");
    }
}

public sealed class CreatePayrollRecordRequestValidator : AbstractValidator<CreatePayrollRecordRequest>
{
    public CreatePayrollRecordRequestValidator()
    {
        RuleFor(x => x.BasicSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Allowances).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Deductions).GreaterThanOrEqualTo(0);
        RuleFor(x => x)
            .Must(x => x.Deductions <= (x.BasicSalary + x.Allowances) * 0.5m)
            .WithMessage("Total deductions cannot exceed 50% of gross monthly pay.");
    }
}
