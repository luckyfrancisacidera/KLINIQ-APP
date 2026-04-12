using Kliniq.Domain.Common;

namespace Kliniq.Domain.Entities
{
    public class Practioner : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public string LicenseNumber { get; private set; } = string.Empty;
        public string Specialty { get; private set; } = string.Empty;

        private Practioner() { }

        public Practioner(Guid userId, string licenseNumber, string specialty)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            LicenseNumber = licenseNumber;
            Specialty = specialty;
        }


    }
}
