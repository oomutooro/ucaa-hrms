using FluentValidation;
using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Validation;

public sealed class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
    }
}

public sealed class CreatePayrollRecordRequestValidator : AbstractValidator<CreatePayrollRecordRequest>
{
    public CreatePayrollRecordRequestValidator()
    {
        RuleFor(x => x.BasicSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Allowances).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Deductions).GreaterThanOrEqualTo(0);
    }
}
