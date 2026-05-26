using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Features.Patients.Mappings
{
    public static class PatientMappings
    {
        public static PatientDto ToDto(this Patient p) => new(
            p.Id,
            p.UserId,
            p.Name.FirstName,
            p.Name.LastName,
            p.DateOfBirth,
            p.Age,
            p.Gender.ToString(),
            p.Address.Street,
            p.Address.City,
            p.Address.Country,
            p.PhoneNumber,
            p.EmergencyContact
        );
    }
}