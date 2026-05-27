using FluentValidation;
using Kliniq.Application.Common.Models;
using Kliniq.Application.Common.Validation;

namespace Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest
{
    public class SubmitAccountRequestCommandValidator : AbstractValidator<SubmitAccountRequestCommand>
    {
        public SubmitAccountRequestCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.LicenseNumber)
                .NotEmpty().WithMessage("License number is required.")
                .MaximumLength(150).WithMessage("License number cannot exceed 150 characters.");

            RuleFor(x => x.Specializations)
                .NotNull().WithMessage("At least one specialization is required.")
                .Must(s => s != null && s.Count >= 1).WithMessage("At least one specialization mmust be required.");

            RuleForEach(x => x.Specializations)
                .NotEmpty().WithMessage("Specialization cannot be empty.")
                .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(200).WithMessage("Street must not exceed 200 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters");

            RuleFor(x => x.ClinicName)
                .NotEmpty().WithMessage("Clinic name is required.")
                .MaximumLength(200).WithMessage("Clinic name must not exceed 200 characters.");

            RuleFor(x => x.ClinicLatitude)
                .NotNull().WithMessage("Clinic latitude is required.")
                .InclusiveBetween(-90, 90).WithMessage("Clinic latitude must be between -90 and 90.");

            RuleFor(x => x.ClinicLongitude)
                .NotNull().WithMessage("Clinic longitude is required.")
                .InclusiveBetween(-180, 180).WithMessage("Clinic longitude must be between -180 and 180.");

            RuleFor(x => x.PrcLicense)
                .ApplyFileUploadRules("PRC License / PRC ID");

            RuleFor(x => x.GovernmentId)
                .ApplyFileUploadRules("Valid Government ID");

            RuleFor(x => x.ProfessionalPhoto)
                .ApplyFileUploadRules("Professional Photo / Profile Pic");

            RuleFor(x => x.Cv)
                .ApplyFileUploadRules("CV (Curriculum Vitae)");
        }
    }
}
