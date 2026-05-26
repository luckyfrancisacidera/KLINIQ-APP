namespace Kliniq.Application.Features.Patients.DTOs
{
    public sealed record PatientDto(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        int Age,
        string Gender,
        string Street,
        string City,
        string Country,
        string? PhoneNumber,
        string? EmergencyContact
    );
}
