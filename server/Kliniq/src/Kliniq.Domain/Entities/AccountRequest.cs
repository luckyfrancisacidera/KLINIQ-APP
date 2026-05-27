using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class AccountRequest : AuditableEntity
    {
        public FullName Name { get; private set; } = null!;
        public string Email { get; private set; } = string.Empty;
        public string LicenseNumber { get; private set; } = string.Empty;

        private string _specializations = string.Empty;
        public string SpecializationsRaw
        {
            get => _specializations;
            private set => _specializations = value;
        }

        public IReadOnlyList<string> Specializations =>
            _specializations.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList().AsReadOnly();
        
        public Address Address { get; private set; } = null!;

        public string ClinicName { get; private set; } = string.Empty;
        public GeoLocation ClinicLocation { get; private set; } = null!;

        public string PrcLicensePath { get; private set; } = string.Empty;
        public string GovernmentIdPath { get; private set; } = string.Empty;
        public string ProfessionalPhotoPath { get; private set; } = string.Empty;
        public string CvPath { get; private set; } = string.Empty;

        public AccountRequestStatus Status { get; private set; }
        public string? AdminNote { get; private set; }

        public string? InvitationToken { get; private set; }
        public DateTime? InvitationExpiresAt { get; private set; }
        public bool IsInvitationUsed { get; private set; }

        private AccountRequest() { }

        public AccountRequest(
            FullName name,
            string email,
            string licenseNumber,
            IReadOnlyList<string> specialization,
            Address address,
            string clinicName,
            GeoLocation clinicLocation,
            string prcLicensePath,
            string governmentIdPath,
            string professionalPhotoPath,
            string cvPath
            )
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is Required");

            if(specialization is null || specialization.Count == 0 )
                throw new DomainException("At least one specialization is required for doctor account requests");

            if(specialization.Any(s => string.IsNullOrWhiteSpace(s)))
                throw new DomainException("Specialization cannot contain empty values");

            Id = Guid.NewGuid();
            Email = email;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LicenseNumber = licenseNumber;
            _specializations = string.Join(",", specialization.Select(s => s.Trim()));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            ClinicName = clinicName;
            ClinicLocation = clinicLocation ?? throw new ArgumentNullException(nameof(clinicLocation));
            PrcLicensePath = prcLicensePath;
            GovernmentIdPath = governmentIdPath;
            ProfessionalPhotoPath = professionalPhotoPath;
            CvPath = cvPath;
            Status = AccountRequestStatus.Pending;
        }

        public void Approve(string? adminNote = null)
        {
            if (Status != AccountRequestStatus.Pending)
                throw new DomainException("Requeest already processed");

            Status = AccountRequestStatus.Approved;
            AdminNote = adminNote;

            InvitationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "_")
                .TrimEnd('=');

            InvitationExpiresAt = DateTime.UtcNow.AddDays(7);
            IsInvitationUsed = false;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Reject(string? adminNote = null)
        {
            if(Status != AccountRequestStatus.Pending)
                throw new DomainException("Request already processed");
            
            if(string.IsNullOrWhiteSpace(adminNote))
                throw new DomainException("Admin note is required for rejecting a request");

            Status = AccountRequestStatus.Rejected;
            AdminNote = adminNote;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void MarkInvitationUsed()
        {
            if (IsInvitationUsed)
                throw new DomainException("Invitation has already been used.");

            if (InvitationExpiresAt < DateTime.UtcNow)
                throw new DomainException("Invitation has expired");

            IsInvitationUsed = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
