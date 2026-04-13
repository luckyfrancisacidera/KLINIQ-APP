using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Entities
{
    public class AccountRequest : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public UserRole RequestRole { get; private set; }
        public string? LicenseNumber { get; private set; }
        public string? Specialization { get; private set; }
        public AccountRequestStatus Status { get; private set; }
        public string? AdminNote { get; private set; }

        private AccountRequest() { }

        public AccountRequest(
            Guid userId,
            UserRole requestedRole,
            string? licenseNumber,
            string? specialization)
        {
             if(requestedRole == UserRole.Practitioner && string.IsNullOrWhiteSpace(LicenseNumber))
                throw new DomainException("License number is required for practitioner account requests");

             Id = Guid.NewGuid();
            UserId = userId;
            RequestRole = requestedRole;
            LicenseNumber = licenseNumber;
            Specialization = specialization;

            Status = AccountRequestStatus.Pending;
        }

        public void Approve(string? adminNote = null)
        {
            if (Status != AccountRequestStatus.Pending)
                throw new DomainException("Requeest already processed");

            Status = AccountRequestStatus.Approved;
            AdminNote = adminNote;
        }

        public void Reject(string? adminNote = null)
        {
            if(Status != AccountRequestStatus.Pending)
                throw new DomainException("Request already processed");

            Status = AccountRequestStatus.Rejected;
            AdminNote = adminNote;
        }
    }
}
