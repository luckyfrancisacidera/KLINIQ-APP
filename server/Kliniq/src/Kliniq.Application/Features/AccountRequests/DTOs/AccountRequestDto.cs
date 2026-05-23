namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public record AccountRequestDto 
    {
        public Guid Id { get; init; }

        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;
        public string LicenseNumber { get; init; } = string.Empty;

        public IReadOnlyList<string> Specializations { get; init; } = [];

        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        
        public double ClinicLatitude { get; init; }
        public double ClinicLongitude { get; init; }

        public string PrcLicensePath { get; init; } = string.Empty;
        public string GovernmentIdPath { get; init; } = string.Empty;
        public string ProfessionalPhotoPath { get; init; } = string.Empty;
        public string CvPath { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;
        public string? AdminNote { get; init; }

        public bool IsInvitationUsed { get; init; }
        public DateTime? InvitationExpiresAt { get; init; }

        public DateTime CreatedAtUtc { get; init; }
    }
}
