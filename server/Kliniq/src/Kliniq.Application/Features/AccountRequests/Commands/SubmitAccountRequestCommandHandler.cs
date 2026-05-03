using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Application.Features.AccountRequests.Mappings;
using Kliniq.Domain.Entities;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands
{
    public class SubmitAccountRequestCommandHandler : IRequestHandler<SubmitAccountRequestCommand, AccountRequestDto>
    {
        private readonly IAccountRequestRepository _repository;
        private readonly IFileStorageService _fileStorage;
        private readonly IUnitOfWork _unitOfWork;

        public SubmitAccountRequestCommandHandler(IAccountRequestRepository repository, IFileStorageService fileStorage, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccountRequestDto> Handle(SubmitAccountRequestCommand request, CancellationToken cancellationToken)
        {
            var exists = await _repository.ExistsPendingEmailAsync(request.Email, cancellationToken);

            if(exists) throw new InvalidOperationException("A pending account request with the same email already exists.");

            //upload documents 
            var prcIdPath = await _fileStorage.UploadAsync(
                request.PrcId.Content,
                request.PrcId.FileName,
                "account-requests/prc-ids",
                cancellationToken);

            var boardCertPath = await _fileStorage.UploadAsync(
                request.BoardCertificate.Content,
                request.BoardCertificate.FileName,
                "account-requests/board-certificates",
                cancellationToken);

            var diplomaPath = await _fileStorage.UploadAsync(
                request.MedicalDiploma.Content,
                request.MedicalDiploma.FileName,
                "account-requests/medical-diplomas",
                cancellationToken);

            var cogsCertPath = await _fileStorage.UploadAsync(
                request.CertificateOfGoodStanding.Content,
                request.CertificateOfGoodStanding.FileName,
                "account-requests/good-standing-certs",
                cancellationToken);

            var name = new FullName(request.FirstName, request.LastName);
            var address = new Address(request.Street, request.City, request.Country);

            var accountRequest = new AccountRequest(
                name,
                request.Email,
                request.Specialization,
                address,
                prcIdPath,
                boardCertPath,
                diplomaPath,
                cogsCertPath);
            
            await _repository.AddAsync(accountRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return accountRequest.ToDto();
        }
    }
}
