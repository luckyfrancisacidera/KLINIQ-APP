using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Auth.DTOs;
using Kliniq.Domain.Entities;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword
{
    public class SetPractitionerPasswordCommandHandler : IRequestHandler<SetPractitionerPasswordCommand, RegisterPractitionerResponseDto>
    {
        private readonly IAuthService _authService;
        private readonly IAccountRequestRepository _accountRequestRepository;
        private readonly IPractitionerRepository _practitionerRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetPractitionerPasswordCommandHandler(
            IAuthService authService,
            IAccountRequestRepository accountRequestRepository,
            IPractitionerRepository practitionerRepository,
            IClinicRepository clinicRepository,
            IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _accountRequestRepository = accountRequestRepository;
            _practitionerRepository = practitionerRepository;
            _clinicRepository = clinicRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterPractitionerResponseDto> Handle(
            SetPractitionerPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var accountRequest = await _accountRequestRepository
                .GetByInvitationTokenAsync(request.InvitationToken, cancellationToken);

            if (accountRequest is null)
                throw new InvalidOperationException("Invalid or expired invitation token");

            var clinic = await _clinicRepository
                .GetByIdAsync(request.ClinicId, cancellationToken);

            if (clinic is null)
                throw new InvalidOperationException("Clinic not found");

            var result = await _authService.RegisterAsync(
                accountRequest.Email,
                request.Password,
                "Practitioner",
                cancellationToken);

            var practitioner = new Practitioner(
                Guid.Parse(result.UserId),          
                new FullName(
                    accountRequest.Name.FirstName,
                    accountRequest.Name.LastName),
                clinic,
                accountRequest.LicenseNumber ?? string.Empty, 
                accountRequest.Specialization ?? string.Empty);

            await _practitionerRepository.AddAsync(practitioner, cancellationToken);

            accountRequest.MarkInvitationUsed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterPractitionerResponseDto
            {
                Email = accountRequest.Email,
                Message = "Password set successfully. You can now log in."
            };
        }
    }
}
