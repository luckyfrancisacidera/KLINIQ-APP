using Kliniq.Domain.Common;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class Practitioner : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public FullName Name { get; private set; } = null!;
        public string LicenseNumber { get; private set; } = string.Empty;
        public string Specialization { get; private set; } = string.Empty;

        public Guid? ClinicID { get; private set; }
        public Clinic? Clinic { get; set; }

        private readonly List<Schedule> _schedules = new();
        public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

        private readonly List<Appointment> _appointments = new();
        public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

        private Practitioner() { }

        public Practitioner(Guid userId, FullName name, string licenseNumber, string specialization)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LicenseNumber = licenseNumber;
            Specialization = specialization;
        }

        public void UpdateProfile(FullName name, string licenseNumber, string specialization)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrWhiteSpace(licenseNumber))
                throw new DomainException("License number is required.");

            if(string.IsNullOrWhiteSpace(specialization))
                throw new DomainException("Specialization is required.");

            LicenseNumber = licenseNumber;
            Specialization = specialization;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void AssignClinic(Clinic clinic)
        {
            if (ClinicID is not null)
                throw new DomainException("Practitioner is already assigned to a clinic.");

            Clinic = clinic;
            ClinicID = clinic.Id;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UnAssignClinic(Clinic clinic)
        {
            if (ClinicID is null)
                throw new DomainException("Practitioner is not assigned to any clinic.");
            
            Clinic = null;
            ClinicID = null;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
