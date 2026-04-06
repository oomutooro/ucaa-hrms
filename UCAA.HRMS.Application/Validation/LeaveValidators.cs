using FluentValidation;
using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Validation;

public sealed class ApplyLeaveRequestValidator : AbstractValidator<ApplyLeaveRequest>
{
    public ApplyLeaveRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
    }
}

public sealed class ReviewLeaveRequestValidator : AbstractValidator<ReviewLeaveRequest>
{
    public ReviewLeaveRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
