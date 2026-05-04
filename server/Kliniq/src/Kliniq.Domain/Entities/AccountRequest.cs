using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class AccountRequest : AuditableEntity
    {
        public FullName Name { get; private set; } = null!;
        public string Email { get; private set; } = string.Empty;
        public string Specialization { get; private set; } = string.Empty;

        public Address Address { get; private set; } = null!;

        public string PrcIdPath { get; private set; } = string.Empty;
        public string BoardCertificatePath { get; private set; } = string.Empty;
        public string MedicalDiplomaPath { get; private set; } = string.Empty;
        public string CertificateOfGoodStandingPath { get; private set; } = string.Empty;

        public AccountRequestStatus Status { get; private set; }
        public string? AdminNote { get; private set; }

        public string? InvitationToken { get; private set; }
        public DateTime? InvitationExpiresAt { get; private set; }
        public bool IsInvitationUsed { get; private set; }

        private AccountRequest() { }

        public AccountRequest(
            FullName name,
            string email,
            string specialization,
            Address address,
            string prcIdPath,
            string boardCertificatePath,
            string medicalDiplomaPath,
            string certificateOfGoodStandingPath
            )
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is Required");

            if(string.IsNullOrWhiteSpace(specialization))
                throw new DomainException("Specialization is required for doctor account requests");

            Id = Guid.NewGuid();
            Email = email;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Specialization = specialization;
            Address = address ?? throw new ArgumentException(nameof(address));
            PrcIdPath = prcIdPath;
            BoardCertificatePath = boardCertificatePath;
            MedicalDiplomaPath = medicalDiplomaPath;
            CertificateOfGoodStandingPath = certificateOfGoodStandingPath;
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
        }

        public void Reject(string? adminNote = null)
        {
            if(Status != AccountRequestStatus.Pending)
                throw new DomainException("Request already processed");
            
            if(string.IsNullOrWhiteSpace(adminNote))
                throw new DomainException("Admin note is required for rejecting a request");

            Status = AccountRequestStatus.Rejected;
            AdminNote = adminNote;
        }

        public void MarkInvitationUsed()
        {
            if (IsInvitationUsed)
                throw new DomainException("Invitation has already been used.");

            if (InvitationExpiresAt < DateTime.UtcNow)
                throw new DomainException("Invitation has expired");
            IsInvitationUsed = true;

        }
    }
}
