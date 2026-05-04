using Kliniq.Application.Common.Models;
using Kliniq.Application.Features.AccountRequests.DTOs;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest
{
    public class SubmitAccountRequestCommand : IRequest<AccountRequestDto>
    {
        //personal-info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        //address
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        //document uploads
        public FileUpload PrcId { get; set; } = null!;
        public FileUpload BoardCertificate { get; set; } = null!;
        public FileUpload MedicalDiploma { get; set; } = null!;
        public FileUpload CertificateOfGoodStanding { get; set; } = null!;
    }
}
