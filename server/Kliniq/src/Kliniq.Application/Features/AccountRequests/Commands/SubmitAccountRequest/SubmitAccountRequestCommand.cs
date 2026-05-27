using Kliniq.Application.Common.Models;
using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest
{
    public class SubmitAccountRequestCommand : IRequest<Result<AccountRequestDto>>
    {
        //personal-info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public List<string> Specializations { get; set; } = [];

        //address
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public string ClinicName { get; set; } = string.Empty;
        public double ClinicLatitude { get; set; }
        public double ClinicLongitude { get; set; }

        //document uploads
        public FileUpload PrcLicense { get; set; } = default!;
        public FileUpload GovernmentId { get; set; } = default!;
        public FileUpload ProfessionalPhoto { get; set; } = default!;
        public FileUpload Cv { get; set; } = default!;
    }
}
