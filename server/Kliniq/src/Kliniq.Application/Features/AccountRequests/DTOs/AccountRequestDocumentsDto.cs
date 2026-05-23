namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public record AccountRequestDocumentsDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public IReadOnlyList<string> Specializations { get; init; } = [];
        public string PrcLicensePath { get; init; } = string.Empty;
        public string GovernmentIdPath { get; init; } = string.Empty;
        public string ProfessionalPhotoPath { get; init; } = string.Empty;
        public string CvPath { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAtUtc { get; init; } 
    }
}
