using FluentValidation;

namespace Kliniq.Application.Features.Patients.Commands.UpdatePatient
{
    public sealed class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.PatientId).NotEmpty().WithMessage("Patient ID is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required.")
                .MaximumLength(200).WithMessage("Street cannot exceed 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

            RuleFor(x => x.PhoneNumber)
              .NotEmpty().WithMessage("Phone number is required.")
              .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
              .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
              .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.EmergencyContact)
                .MaximumLength(100).WithMessage("Emergency contact cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.EmergencyContact));
        }
    }
}
