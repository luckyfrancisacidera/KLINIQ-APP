using FluentValidation;
using Kliniq.Application.Common.Models;

namespace Kliniq.Application.Common.Validation
{
    public static class FileUploadValidator
    {
        private static readonly string[] AllowedContentTypes = ["application/pdf", "image/jpeg", "image/png"];

        private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public static IRuleBuilderOptions<T, FileUpload?> ApplyFileUploadRules<T>(
            this IRuleBuilder<T, FileUpload?> rule,
            string fieldLabel)
        {
            return rule
                .NotNull()
                    .WithMessage($"{fieldLabel} is required.")
                .Must(f => f!.Size > 0)
                    .WithMessage($"{fieldLabel} file is empty.")
                .Must(f => f!.Size <= MaxFileSizeBytes)
                    .WithMessage($"{fieldLabel} must not exceed 5 MB.")
                .Must(f => AllowedContentTypes.Contains(f!.NormalizedContentType))
                    .WithMessage($"{fieldLabel} must be pdf, jpg, jpeg, or png.")
                .Must(f => AllowedExtensions.Contains(f!.Extension))
                    .WithMessage($"{fieldLabel} has an invalid file extension.")
                .Must(f => FileSignatureValidator.IsValidSignature(f!.Content, f.Extension))
                    .WithMessage($"{fieldLabel} file content does not match its declared type.");
        }
    }
}