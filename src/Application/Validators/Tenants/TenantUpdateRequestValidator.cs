using FluentValidation;
using StudentApi.Application.DTOs.Tenants;

namespace StudentApi.Application.Validators.Tenants;

public sealed class TenantUpdateRequestValidator : AbstractValidator<TenantUpdateRequest>
{
    public TenantUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);
    }
}
