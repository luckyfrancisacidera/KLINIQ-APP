using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Common.Settings;
using Kliniq.Domain.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest
{
    public class ApproveAccountRequestCommandHandler : IRequestHandler<ApproveAccountRequestCommand, Result>
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

        public async Task<Result> Handle(ApproveAccountRequestCommand request, CancellationToken cancellationToken)
        {
            var accountRequest = await _repository.GetByIdAsync(request.AccountRequestId, cancellationToken);

            if (accountRequest is null)
                return Result.Failure(Error.NotFound("AccountRequest.NotFound", "Account request not found"));

            accountRequest.Approve(request.AdminNote);

            var inviteLink = $"{_appSettings.BaseUrl.TrimEnd('/')}/set-password?token={Uri.EscapeDataString(accountRequest.InvitationToken!)}";

            var subject = "You're invited to join KLINIQ";
            var firstName = WebUtility.HtmlEncode(accountRequest.Name.FirstName);
            var lastName = WebUtility.HtmlEncode(accountRequest.Name.LastName);
            var email = WebUtility.HtmlEncode(accountRequest.Email);
            var specializations = WebUtility.HtmlEncode(string.Join(", ", accountRequest.Specializations));
            var safeInviteLink = WebUtility.HtmlEncode(inviteLink);
            var body = $"""
                <h2>Welcome to KLINIQ, Dr. {firstName} {lastName}</h2>
                <p>Your account request has been approved</p>
                <p>Click the link below to complete your registration
                   You only need to set your password.</p>
                
                <br/>

                <a href="{safeInviteLink}"
                   style="background:#0066cc;color:white;padding:12px 24px;    
                          border-radius:6px;text-decoration:none;">
                Complete your registration
                </a>

                <br/><br/>
                <p><strong>This link expires in 7 days.</strong></p>
                <p>Your registered details:</p>
                <ul>
                    <li>Email: {email}</li>
                    <li>Specialization: {specializations}</li>
                </ul>
                """;

            await _emailService.SendEmailAsync(accountRequest.Email, subject, body, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }

    }
}
