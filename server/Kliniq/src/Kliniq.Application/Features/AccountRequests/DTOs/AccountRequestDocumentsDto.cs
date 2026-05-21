namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public record AccountRequestDocumentsDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Specialization { get; init; } = string.Empty;
        public string PrcIdPath { get; init; } = string.Empty;
        public string BoardCertificatePath { get; init; } = string.Empty;
        public string MedicalDiplomaPath { get; init; } = string.Empty;
        public string CertificateOfGoodStandingPath { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAtUtc { get; init; } 
    }
}
