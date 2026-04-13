using Kliniq.Domain.Common;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class Practioner : AuditableEntity
    {
        public Guid UserId { get; private set; }

        public FullName Name { get; private set; } = null!;
        public string LicenseNumber { get; private set; } = string.Empty;
        public string Specialization { get; private set; } = string.Empty;

        public Guid ClinicID { get; private set; }
        public Clinic? Clinic { get; set; }

        private Practioner() { }

        public Practioner(Guid userId, FullName name, Clinic clinicId, string licenseNumber, string specialty)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Clinic = clinicId;
            LicenseNumber = licenseNumber;
            Specialty = specialty;
        }


    }
}
