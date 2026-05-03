using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Domain.Entities;
using System.ComponentModel;
using System.Diagnostics;

namespace Kliniq.Application.Features.AccountRequests.Mappings
{
    public static class AccountRequestMappings
    {
        //full detail
        public static AccountRequestDto ToDto(this AccountRequest request) => new()
        {
            Id = request.Id,
            FirstName = request.Name.FirstName,
            LastName = request.Name.LastName,
            Email = request.Email,
            Specialization = request.Specialization,
            Street = request.Address.Street,
            City = request.Address.City,
            Country = request.Address.Country,
            PrcIdPath = request.PrcIdPath,
            BoardCertificatePath = request.BoardCertificatePath,
            MedicalDiplomaPath = request.MedicalDiplomaPath,
            CertificateOfGoodStandingPath = request.CertificateOfGoodStandingPath,
            Status = request.Status.ToString(),
            AdminNote = request.AdminNote,
            IsInvitationUsed = request.IsInvitationUsed,
            InvitatioNExpiresAt = request.InvitationExpiresAt,
            CreatedAtUtc = request.CreatedAtUtc
        };

        //summary
        public static AccountRequestSummaryDto ToSummaryDto(this AccountRequest request) => new()
        {
            Id = request.Id,
            FirstName = request.Name.FirstName,
            LastName = request.Name.LastName,
            Email = request.Email,
            Specialization = request.Specialization,
            Street = request.Address.Street,
            City = request.Address.City,
            Country = request.Address.Country,
            Status = request.Status.ToString(),
            CreatedAtUtc = request.CreatedAtUtc
        };

        //documents only
        public static AccountRequestDocumentsDto ToDocumentsDto(this AccountRequest request) => new()
        {
            Id = request.Id,
            FirstName = request.Name.FirstName,
            LastName = request.Name.LastName,
            Email = request.Email,
            Specialization = request.Specialization,
            PrcIdPath = request.PrcIdPath,
            BoardCertificatePath = request.BoardCertificatePath,
            MedicalDiplomaPat = request.MedicalDiplomaPath,
            CertificateOfGoodStandingPath = request.CertificateOfGoodStandingPath,
            Status = request.Status.ToString(),
            CreatedAtUtc = request.CreatedAtUtc
        };

        //list-helpers
        public static IEnumerable<AccountRequestDto> ToDtoList(
            this IEnumerable<AccountRequest> requests)
            => requests.Select(r => r.ToDto());

        public static IEnumerable<AccountRequestSummaryDto> ToSummaryDtoList(this IEnumerable<AccountRequest> requests)
            => requests.Select(r => r.ToSummaryDto());
    }
}
