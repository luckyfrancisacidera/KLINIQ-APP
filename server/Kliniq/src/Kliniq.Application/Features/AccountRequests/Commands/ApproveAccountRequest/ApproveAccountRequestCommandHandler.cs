using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Common.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest
{
    public class ApproveAccountRequestCommandHandler : IRequestHandler<ApproveAccountRequestCommand, bool>
    {
        private readonly IAccountRequestRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        public ApproveAccountRequestCommandHandler(IAccountRequestRepository repository, IEmailService emailService, IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            _repository = repository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public async Task<bool> Handle(ApproveAccountRequestCommand request, CancellationToken cancellationToken)
        {
            var accountRequest = await _repository.GetByIdAsync(request.AccountRequestId, cancellationToken);

            if (accountRequest is null)
                throw new InvalidOperationException("Account request not found.");

            accountRequest.Approve(request.AdminNote);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var inviteLink = $"{_appSettings.BaseUrl}/register/practitioner?token={accountRequest.InvitationToken}";

            var subject = "You're invited to join Kliniq";
            var body = $"""
                <h2>Welcome to Kliniq Dr.{accountRequest.Name.FirstName} {accountRequest.Name.LastName}"</h2>
                <p>Your account request has been appproved</p>
                <p>Click the link below to complete your registration
                   You only need to set your password.</p>
                
                <br/>

                <a href="{inviteLink}"
                   style="background:##0066cc;color:white;padding:12px 24px;    
                          border-radius:6px;text-decoration-none;">
                Complete your registration
                </a>

                <br/><br/>
                <p><strong>This link expires in 7 days.</strong></p>
                <p>Your registered details:</p>
                <ul>
                    <li>Email: {accountRequest.Email} </li>
                    <li>Specialization: {accountRequest.Specialization} </li>
                </ul>
                """;

            await _emailService.SendEmailAsync(accountRequest.Email, subject, body, cancellationToken);

            return true;

        }

    }
}
