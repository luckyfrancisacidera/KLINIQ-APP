using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.Auth.Dto;
using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.ValueObjects;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(
            IAuthService authService,
            IJwtTokenService jwtTokenService,
            IPatientRepository patientRepository,
            IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                "Patient",
                cancellationToken);

            var patient = new Patient
            (
                Guid.Parse(authResult.UserId),
                new FullName(request.FirstName, request.LastName),
                request.DateOfBirth,
                request.Gender,
                new Address(request.Street, request.City, request.Country),
                request.PhoneNumber,
                request.EmergencyContact
            );

            await _patientRepository.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var accessToken = _jwtTokenService.GenerateAccessToken(authResult.Email, authResult.UserId, "Patient");

            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var refreshTokenHash = _jwtTokenService.HashRefreshToken(refreshToken);

            await _authService.SaveRefreshTokenAsync(authResult.UserId, refreshTokenHash, cancellationToken);

            return Result.Success( new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAtUtc = _jwtTokenService.GetAccessTokenExpiry(),
                UserId = authResult.UserId,
                Email = authResult.Email    ,
                Role = "Patient"
            });
        }
    }
}
