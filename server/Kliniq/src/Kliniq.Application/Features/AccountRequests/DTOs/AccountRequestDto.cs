namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public record AccountRequestDto 
    {
        public Guid Id { get; init; }

        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;
        public string LicenseNumber { get; init; } = string.Empty;
        public string Specialization { get; init; } = string.Empty;
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;

        public string PrcIdPath { get; init; } = string.Empty;
        public string BoardCertificatePath { get; init; } = string.Empty;
        public string MedicalDiplomaPath { get; init; } = string.Empty;
        public string CertificateOfGoodStandingPath { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;
        public string? AdminNote { get; init; }

        public bool IsInvitationUsed { get; init; }
        public DateTime? InvitatioNExpiresAt { get; init; }

        public DateTime CreatedAtUtc { get; init; }
    }
}
