using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.AccountRequests.DTOs;
using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Domain.Entities;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.RegisterPractitioner
{
    public class RegisterPractitionerHandler : IRequestHandler<RegisterPractitionerCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAccountRequestRepository _accountRequestRepository;
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterPractitionerHandler(
            IAuthService authService, 
            IJwtTokenService jwtTokenService, 
            IAccountRequestRepository accountRequestRepository,
            IPractitionerRepository practitionerRepository,
            IClinicRepository clinicRepository,
            IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _accountRequestRepository = accountRequestRepository;
            _practitionerRepository = practitionerRepository;
            _clinicRepository = clinicRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<AuthResponseDto> Handle(RegisterPractitionerCommand request, CancellationToken cancellationToken)
        {
            //validate the invitation token
            var accountRequest = await _accountRequestRepository.GetByInvitationTokenAsync(request.InvitationToken, cancellationToken);

            if (accountRequest is null)
                throw new InvalidOperationException("Invalid or expired invitation token.");

            var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, cancellationToken);

            if (clinic is null)
                throw new InvalidOperationException("Clinic not found.");

            var result = await _authService.RegisterAsync(
                accountRequest.Email,
                request.Password,
                "Practitioner",
                cancellationToken);

            var practitioner = new Practitioner
            (
                result.userId,
                new FullName(accountRequest.FirstName, accountRequest.LastName),
                clinic,
                accountRequest.Specialization
            );

            await _practitionerRepository.AddAsync(practitioner, cancellationToken);

            accountRequest.MarkInvitationUsed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenService.GenerateToken(result.userId, result.email, "Practitioner");

            return new AuthResponseDto
            {
                Token = token,
                UserId = result.userId.ToString(),
                Email = result.email,
                Role = "Practitioner"
            };
        }
    }
}
