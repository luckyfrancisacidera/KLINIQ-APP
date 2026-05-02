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

        private readonly List<Schedule> _schedules = new();
        public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

        private readonly List<Appointment> _appointments = new();
        public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

        private Practioner() { }

        public Practioner(Guid userId, FullName name, Clinic clinic, string licenseNumber, string specialization)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Clinic = clinic;
            ClinicID = clinic.Id;
            LicenseNumber = licenseNumber;
            Specialization = specialization;
        }


    }
}
