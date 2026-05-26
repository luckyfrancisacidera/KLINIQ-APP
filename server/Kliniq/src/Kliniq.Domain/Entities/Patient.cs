using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class Patient : AuditableEntity
    {
        public Guid UserId { get; private set; }
       
        public FullName Name { get; private set; } = null!;
        public DateTime DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public Address Address { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }
        public string? EmergencyContact { get; private set; }

        private readonly List<Appointment> _appointments = new();
        public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

        public int Age => CalculateAge();

        private Patient() { }

        public Patient(
            Guid userId,
            FullName name,
            DateTime dateOfBirth,
            Gender gender,
            Address address,
            string? phoneNumber,
            string? emergencyContact
            )            
        {
            if (dateOfBirth >= DateTime.UtcNow.Date)
                throw new ArgumentException("Date of birth cannot be today or in the future");
            
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PhoneNumber = phoneNumber;
            EmergencyContact = emergencyContact;
        }

        public void UpdateProfile(FullName name, Address address, string? phoneNumber, string? emergencyContact)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PhoneNumber = phoneNumber;
            EmergencyContact = emergencyContact;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateName(FullName newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateAddress(Address newAddress)
        {
            Address = newAddress;
            UpdatedAtUtc = DateTime.UtcNow;
        }
        public void UpdatePhoneNumber(string newPhoneNumber)
        {
            PhoneNumber = newPhoneNumber;
            UpdatedAtUtc = DateTime.UtcNow;
        }
        public void UpdateEmergencyContact(string? newEmergencyContact)
            {
                EmergencyContact = newEmergencyContact;
                UpdatedAtUtc = DateTime.UtcNow;
        }

        private int CalculateAge()
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
