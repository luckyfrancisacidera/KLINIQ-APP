using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Auth.DTOs;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword
{
    public class SetPractitionerPasswordCommandHandler : IRequestHandler<SetPractitionerPasswordCommand, Result<RegisterPractitionerResponseDto>>
    {
        private readonly IAuthService _authService;
        private readonly IAccountRequestRepository _accountRequestRepository;
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetPractitionerPasswordCommandHandler(
            IAuthService authService,
            IAccountRequestRepository accountRequestRepository,
            IPractitionerRepository practitionerRepository,
            IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _accountRequestRepository = accountRequestRepository;
            _practitionerRepository = practitionerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RegisterPractitionerResponseDto>> Handle(
            SetPractitionerPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var accountRequest = await _accountRequestRepository
                .GetByInvitationTokenAsync(request.InvitationToken, cancellationToken);

            if (accountRequest is null)
                return Result.Failure<RegisterPractitionerResponseDto>
                    (Error.NotFound("Auth.InvalidToken", "Invalid or expired invitation token"));

            if(accountRequest.IsInvitationUsed)
                return Result.Failure<RegisterPractitionerResponseDto>
                    (Error.Conflict("Auth.TokenUsed", "This invitation token has already been used"));

            var authResult = await _authService.RegisterAsync(
                accountRequest.Email,
                request.Password,
                "Practitioner",
                cancellationToken);

            if (!authResult.Succeeded)
                return Result.Failure<RegisterPractitionerResponseDto>
                    (Error.Conflict("Auth.EmailTaken", "An account with this email already exists"));


            var practitioner = new Practitioner(
                Guid.Parse(authResult.UserId),          
                new FullName(
                    accountRequest.Name.FirstName,
                    accountRequest.Name.LastName),
                accountRequest.LicenseNumber ?? string.Empty, 
                accountRequest.Specialization ?? string.Empty);

            await _practitionerRepository.AddAsync(practitioner, cancellationToken);

            accountRequest.MarkInvitationUsed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new RegisterPractitionerResponseDto
            {
                Email = accountRequest.Email,
                Message = "Password set successfully. You can now log in with your credentials."
            });
        }
    }
}
