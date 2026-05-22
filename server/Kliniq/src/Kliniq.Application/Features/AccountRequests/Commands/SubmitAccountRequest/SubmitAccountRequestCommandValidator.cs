using FluentValidation;
using Kliniq.Application.Common.Models;
using Kliniq.Application.Common.Validation;

namespace Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest
{
    public class SubmitAccountRequestCommandValidator : AbstractValidator<SubmitAccountRequestCommand>
    {
        private readonly string[] _allowedContentTypes =
        {
            "application/pdf",
            "image/jpeg",
            "image/png"
        };

        private const long MaxFileSizedBytes = 5 * 1024 * 1024; // 5 MB
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

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required.")
                .MaximumLength(150).WithMessage("Specialization cannot exceed 150 characters.");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(200).WithMessage("Street must not exceed 200 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters");

            RuleFor(x => x.PrcId)
                .ApplyFileUploadRules("PRC ID");

            RuleFor(x => x.BoardCertificate)
                .ApplyFileUploadRules("Board Certificate");

            RuleFor(x => x.MedicalDiploma)
                .ApplyFileUploadRules("Medical Diploma");

            RuleFor(x => x.CertificateOfGoodStanding)
                .ApplyFileUploadRules("Certificate of Good Standing");
        }
    }
}
