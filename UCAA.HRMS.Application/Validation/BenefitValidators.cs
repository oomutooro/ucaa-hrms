using FluentValidation;
using UCAA.HRMS.Application.DTOs;
using UCAA.HRMS.Domain.Enums;

namespace UCAA.HRMS.Application.Validation;

public sealed class CreateBenefitPlanRequestValidator : AbstractValidator<CreateBenefitPlanRequest>
{
    public CreateBenefitPlanRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(600);
        RuleFor(x => x.DefaultEmployerContribution).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DefaultEmployeeContribution).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateBenefitPlanRequestValidator : AbstractValidator<UpdateBenefitPlanRequest>
{
    public UpdateBenefitPlanRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(600);
        RuleFor(x => x.DefaultEmployerContribution).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DefaultEmployeeContribution).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateBenefitEnrollmentRequestValidator : AbstractValidator<CreateBenefitEnrollmentRequest>
{
    public CreateBenefitEnrollmentRequestValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.BenefitPlanId).NotEmpty();
        RuleFor(x => x.EmployerContribution)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EmployerContribution.HasValue);
        RuleFor(x => x.EmployeeContribution)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EmployeeContribution.HasValue);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be on or after start date.");
    }
}

public sealed class UpdateBenefitEnrollmentStatusRequestValidator : AbstractValidator<UpdateBenefitEnrollmentStatusRequest>
{
    public UpdateBenefitEnrollmentStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.EndDate)
            .NotNull()
            .When(x => x.Status == BenefitEnrollmentStatus.Terminated)
            .WithMessage("End date is required when status is Terminated.");
    }
}
