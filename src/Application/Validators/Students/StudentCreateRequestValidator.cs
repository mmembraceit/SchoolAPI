using FluentValidation;
using StudentApi.Application.DTOs.Students;

namespace StudentApi.Application.Validators.Students;

public sealed class StudentCreateRequestValidator : AbstractValidator<StudentCreateRequest>
{
    public StudentCreateRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("DateOfBirth is required.")
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("DateOfBirth must be in the past.");
    }
}
