using FluentValidation;
using UCAA.HRMS.Application.DTOs;

namespace UCAA.HRMS.Application.Validation;

public sealed class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.EmployeeId).NotEmpty().MaximumLength(40);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(80);
        RuleFor(x => x.JobLevel).InclusiveBetween(1, 14);

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            .WithMessage("Employee must be at least 18 years old.");

        RuleFor(x => x.DateOfBirth)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-60)))
            .WithMessage("Employee must be below mandatory retirement age (60 years) at appointment.");

        RuleFor(x => x.FirstEmploymentDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("First employment date cannot be in the future.");

        RuleFor(x => x.FirstEmploymentDate)
            .GreaterThan(x => x.DateOfBirth)
            .WithMessage("First employment date must be after date of birth.");

        RuleFor(x => x.FirstEmploymentDate)
            .GreaterThanOrEqualTo(x => x.DateOfBirth.AddYears(18))
            .WithMessage("First employment date must be on or after the 18th birthday.");
    }
}

public sealed class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(80);
        RuleFor(x => x.JobLevel).InclusiveBetween(1, 14);

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            .WithMessage("Employee must be at least 18 years old.");

        RuleFor(x => x.DateOfBirth)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-60)))
            .WithMessage("Employee must be below mandatory retirement age (60 years) at appointment.");

        RuleFor(x => x.FirstEmploymentDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("First employment date cannot be in the future.");

        RuleFor(x => x.FirstEmploymentDate)
            .GreaterThan(x => x.DateOfBirth)
            .WithMessage("First employment date must be after date of birth.");

        RuleFor(x => x.FirstEmploymentDate)
            .GreaterThanOrEqualTo(x => x.DateOfBirth.AddYears(18))
            .WithMessage("First employment date must be on or after the 18th birthday.");
    }
}
