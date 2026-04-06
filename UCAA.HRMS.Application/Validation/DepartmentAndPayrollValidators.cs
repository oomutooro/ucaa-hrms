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
        RuleFor(x => x.TransportAllowance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.HousingAllowance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherAllowance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PayeTax).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PensionDeduction).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LoanDeduction).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherDeduction).GreaterThanOrEqualTo(0);
        RuleFor(x => x)
            .Must(x =>
            {
                var allowances = x.TransportAllowance + x.HousingAllowance + x.OtherAllowance;
                var deductions = x.PayeTax + x.PensionDeduction + x.LoanDeduction + x.OtherDeduction;
                return deductions <= (x.BasicSalary + allowances) * 0.5m;
            })
            .WithMessage("Total deductions cannot exceed 50% of gross monthly pay.");
    }
}
