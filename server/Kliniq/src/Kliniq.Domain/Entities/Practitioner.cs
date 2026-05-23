using Kliniq.Domain.Common;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class Practitioner : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public FullName Name { get; private set; } = null!;
        public string LicenseNumber { get; private set; } = string.Empty;
        


        private string _specialization = string.Empty;
        public string SpecializationsRaw
        {
            get => _specialization;
            private set => _specialization = value;
        }

        public IReadOnlyList<string> Specializations => _specialization.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList().AsReadOnly();

        public Guid? ClinicID { get; private set; }
        public Clinic? Clinic { get; set; }

        private readonly List<Schedule> _schedules = new();
        public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

        private readonly List<Appointment> _appointments = new();
        public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

        private Practitioner() { }

        public Practitioner(Guid userId, FullName name, string licenseNumber, IReadOnlyList<string> specializations)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LicenseNumber = licenseNumber;
            _specialization = string.Join(',', specializations.Select(s => s.Trim()));
        }

        public void UpdateProfile(FullName name, IReadOnlyList<string> specializations)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));

            if (specializations is null || specializations.Count == 0)
                throw new DomainException("At least one specialization is required.");

            if(specializations.Any(s => string.IsNullOrWhiteSpace(s)))
                throw new DomainException("Specialization cannot contain empty values.");

            Name = name;
            _specialization = string.Join(',', specializations.Select(s => s.Trim()));
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
