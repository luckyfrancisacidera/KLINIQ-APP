using FluentValidation;

namespace Kliniq.Application.Features.Practitioners.Commands.UpdatePractitioner
{
    public sealed class UpdatePractitionerCommandValidator : AbstractValidator<UpdatePractitionerCommand>
    {
        public UpdatePractitionerCommandValidator()
        {
            RuleFor(x => x.PractitionerId).NotEmpty().WithMessage("Practitioner ID is required.");  

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required.")
                .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters.");

        }
    }
}
