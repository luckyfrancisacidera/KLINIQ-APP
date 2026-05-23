using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Domain.Entities;

namespace Kliniq.Application.Features.AccountRequests.Mappings
{
    public static class AccountRequestMappings
    {
        public static AccountRequestDto ToDto(this AccountRequest request) => new()
        {
            Id = request.Id,
            FirstName = request.Name.FirstName,
            LastName = request.Name.LastName,
            Email = request.Email,
            LicenseNumber = request.LicenseNumber,
            Specializations = request.Specializations,
            Street = request.Address.Street,
            City = request.Address.City,
            Country = request.Address.Country,
            ClinicLatitude = request.ClinicLocation.Latitude,
            ClinicLongitude = request.ClinicLocation.Longitude,
            PrcLicensePath = request.PrcLicensePath,
            GovernmentIdPath = request.GovernmentIdPath,
            ProfessionalPhotoPath = request.ProfessionalPhotoPath,
            CvPath = request.CvPath,
            Status = request.Status.ToString(),
            AdminNote = request.AdminNote,
            IsInvitationUsed = request.IsInvitationUsed,
            InvitationExpiresAt = request.InvitationExpiresAt,
            CreatedAtUtc = request.CreatedAtUtc
        };

        public static AccountRequestSummaryDto ToSummaryDto(this AccountRequest request) => new()
        {
            Id = request.Id,
            FirstName = request.Name.FirstName,
            LastName = request.Name.LastName,
            Email = request.Email,
            Specializations = request.Specializations,
            Street = request.Address.Street,
            City = request.Address.City,
            Country = request.Address.Country,
            Status = request.Status.ToString(),
            CreatedAtUtc = request.CreatedAtUtc
        };

        public static AccountRequestDocumentsDto ToDocumentsDto(this AccountRequest request) => new()
        {
            Id = request.Id,
            FirstName = request.Name.FirstName,
            LastName = request.Name.LastName,
            Email = request.Email,
            Specializations = request.Specializations,
            PrcLicensePath = request.PrcLicensePath,
            GovernmentIdPath = request.GovernmentIdPath,
            ProfessionalPhotoPath = request.ProfessionalPhotoPath,
            CvPath = request.CvPath,
            Status = request.Status.ToString(),
            CreatedAtUtc = request.CreatedAtUtc
        };

        public static IEnumerable<AccountRequestDto> ToDtoList(
            this IEnumerable<AccountRequest> requests)
            => requests.Select(r => r.ToDto());

        public static IEnumerable<AccountRequestSummaryDto> ToSummaryDtoList(
            this IEnumerable<AccountRequest> requests)
            => requests.Select(r => r.ToSummaryDto());
    }
}