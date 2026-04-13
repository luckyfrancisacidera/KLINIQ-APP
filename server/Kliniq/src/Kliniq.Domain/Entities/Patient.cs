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
            if (dateOfBirth > DateTime.UtcNow)
                throw new ArgumentException("Date of birth cannot be in the future");
            
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Address = address;
            PhoneNumber = phoneNumber;
            EmergencyContact = emergencyContact;
        }


        public int Age => DateTime.UtcNow.Year - DateOfBirth.Year;

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
        public void UpdateEmergencyContact(string newEmergencyContact)
            {
                EmergencyContact = newEmergencyContact;
                UpdatedAtUtc = DateTime.UtcNow;
        }
        public void UpdateName(FullName newName)
        {
            Name = newName;
            UpdatedAtUtc = DateTime.UtcNow;
        }

    }
}
