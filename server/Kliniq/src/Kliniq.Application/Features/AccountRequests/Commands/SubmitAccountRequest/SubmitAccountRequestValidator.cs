using FluentValidation;
using Kliniq.Application.Common.Models;

namespace Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest
{
    public class SubmitAccountRequestValidator : AbstractValidator<SubmitAccountRequestCommand>
    {
        private readonly string[] _allowedContentTypes =
        {
            "application/pdf",
            "image/jpeg",
            "image/png"
        };

        private const long MaxFileSizedBytes = 5 * 1024 * 1024; // 5 MB
        public SubmitAccountRequestValidator()
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
                .NotNull().WithMessage("PRC ID is required")
                .Must(BeValidFile).WithMessage("PRC ID file must be in PDF/JPG/PNG and max 5MB size only");

            RuleFor(x => x.BoardCertificate)
                .NotNull().WithMessage("Board Certificate is required")
                .Must(BeValidFile).WithMessage("Board Certificate file must be in PDF/JPG/PNG and max 5MB size only");

            RuleFor(x => x.MedicalDiploma)
                .NotNull().WithMessage("Medical Diploma is required")
                .Must(BeValidFile).WithMessage("Medical Diploma file must be in PDF/JPG/PNG and max 5MB size only");

            RuleFor(x => x.CertificateOfGoodStanding)
                .NotNull().WithMessage("Certificate of Good Standing is required")
                .Must(BeValidFile).WithMessage("Certificate of Good Standing file must be in PDF/JPG/PNG and max 5MB size only");
        }

        private bool BeValidFile(FileUpload? file)
        {
            if(file is null) return false;
            if(file.Size > MaxFileSizedBytes) return false;
            return _allowedContentTypes.Contains(file.ContentType);

        }
    }
}
