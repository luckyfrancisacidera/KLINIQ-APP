using Kliniq.Domain.Enums;

namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public class AccountRequestDto 
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public string PrcIdPath { get; set; } = string.Empty;
        public string BoardCertificatePath { get; set; } = string.Empty;
        public string MedicalDiplomaPath { get; set; } = string.Empty;
        public string CertificateOfGoodStandingPath { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string? AdminNote { get; set; }

        public bool IsInvitationUsed { get; set; }
        public DateTime? InvitatioNExpiresAt { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
