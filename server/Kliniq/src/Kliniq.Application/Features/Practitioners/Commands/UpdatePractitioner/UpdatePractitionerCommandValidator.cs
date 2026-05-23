using FluentValidation;

namespace Kliniq.Application.Features.Practitioners.Commands.UpdatePractitioner
{
    public sealed class UpdatePractitionerCommandValidator
        : AbstractValidator<UpdatePractitionerCommand>
    {
        public UpdatePractitionerCommandValidator()
        {
            RuleFor(x => x.PractitionerId).NotEmpty().WithMessage("Practitioner ID is required");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Specializations)
                .NotNull().WithMessage("Specializations are required.")
                .Must(s => s != null && s.Count >= 1).WithMessage("At least one specialization is required.");

            RuleForEach(x => x.Specializations)
                .NotEmpty().WithMessage("Specialization entry cannot be blank.")
                .MaximumLength(100).WithMessage("Specialization entry must not exceed 100 characters.");
        }
    }
}