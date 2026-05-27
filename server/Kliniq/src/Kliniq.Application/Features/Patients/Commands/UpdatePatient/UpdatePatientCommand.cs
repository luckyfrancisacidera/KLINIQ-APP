using Kliniq.Application.Features.Patients.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Patients.Commands.UpdatePatient
{
    public sealed record UpdatePatientCommand(
        Guid PatientId,
        string FirstName,
        string LastName,
        string Street,
        string City,
        string Country,
        string? PhoneNumber,
        string? EmergencyContact
    ) : IRequest<Result<PatientDto>>;
}
