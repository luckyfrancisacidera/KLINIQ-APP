using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using System.Runtime.CompilerServices;

namespace Kliniq.Domain.Entities
{
    public class AccountRequest : AuditableEntity
    {
        public string? LicenseNumber { get; private set; }
        public string? Specialization { get; private set; }
        public AccountRequestStatus Status { get; private set; }
        public string? AdminNote { get; private set; }

        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        private AccountRequest() { }

        public AccountRequest(
            string firstName,
            string lastName,
            string email,
            string? licenseNumber,
            string? specialization)
        {
            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new DomainException("License number is required for doctor account requests");

            if(string.IsNullOrWhiteSpace(specialization))
                throw new DomainException("Specialization is required for doctor account requests");

            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
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
