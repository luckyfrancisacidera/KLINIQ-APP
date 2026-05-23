using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Application.Features.AccountRequests.Mappings;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.SubmitAccountRequest
{
    public class SubmitAccountRequestCommandHandler : IRequestHandler<SubmitAccountRequestCommand, Result<AccountRequestDto>>
    {
        private readonly IAccountRequestRepository _repository;
        private readonly IAuthService _authService;
        private readonly IFileStorageService _fileStorage;
        private readonly IUnitOfWork _unitOfWork;

        public SubmitAccountRequestCommandHandler(IAccountRequestRepository repository, IAuthService authService, IFileStorageService fileStorage, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _authService = authService;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AccountRequestDto>> Handle(SubmitAccountRequestCommand request, CancellationToken cancellationToken)
        {
            var emailTaken = await _authService.EmailExistsAsync(request.Email, cancellationToken);

            if(emailTaken)
                return Result.Failure<AccountRequestDto>             
                    (Error.Conflict("AccountRequest.EmailTaken", "An account with this email already exists"));

            var exists = await _repository.ExistsPendingEmailAsync(request.Email, cancellationToken);

            if(exists)
                return Result.Failure<AccountRequestDto>             
                    (Error.Conflict("AccountRequest.EmailExists", "An account request with this email already exists and is pending review"));

            string prcLicensePath, governmentIdPath, professionalPhotoPath, cvPath;

            //upload documents 
            try
            {
                var results = await Task.WhenAll(
                    _fileStorage.UploadAsync(request.PrcLicense!.Content, request.PrcLicense.FileName, "account-requests/prc-licenses", cancellationToken),
                    _fileStorage.UploadAsync(request.GovernmentId!.Content, request.GovernmentId.FileName, "account-requests/government-ids", cancellationToken),
                    _fileStorage.UploadAsync(request.ProfessionalPhoto!.Content, request.ProfessionalPhoto.FileName, "account-requests/professional-photos", cancellationToken),
                    _fileStorage.UploadAsync(request.Cv!.Content, request.Cv.FileName, "account-requests/cvs", cancellationToken)
                );

                (prcLicensePath, governmentIdPath, professionalPhotoPath, cvPath) = (results[0], results[1], results[2], results[3]);
            }
            catch (Exception ex)
            {
                return Result.Failure<AccountRequestDto>(
                    Error.Failure("AccountRequest.UploadFailed", $"Document upload failed: {ex.Message}"));
            }

            var name = new FullName(request.FirstName, request.LastName);
            var address = new Address(request.Street, request.City, request.Country);
            var clinicLocation = new GeoLocation(request.ClinicLatitude, request.ClinicLongitude);

            var accountRequest = new AccountRequest(
                name,
                request.Email,
                request.LicenseNumber,
                request.Specializations.AsReadOnly(),
                address,
                prcLicensePath,
                governmentIdPath,
                professionalPhotoPath,
                cvPath,
                clinicLocation);
            
            await _repository.AddAsync(accountRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(accountRequest.ToDto());
        }
    }
}
