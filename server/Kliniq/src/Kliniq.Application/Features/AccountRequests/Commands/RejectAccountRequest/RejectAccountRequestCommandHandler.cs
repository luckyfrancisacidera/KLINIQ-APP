using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Domain.Common;
using MediatR;
using System.Net;

namespace Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest
{
    public class RejectAccountRequestCommandHandler : IRequestHandler<RejectAccountRequestCommand, Result>
    {
        private readonly IAccountRequestRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public RejectAccountRequestCommandHandler(IAccountRequestRepository repository, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RejectAccountRequestCommand request, CancellationToken cancellationToken)
        {
            var accountRequest = await _repository.GetByIdAsync(request.AccountRequestId, cancellationToken);

            if(accountRequest is null )
                return Result.Failure(Error.NotFound("AccountRequest.NotFound", "Account request not found"));

            accountRequest.Reject(request.AdminNote);

            var subject = "Update on your KLINIQ practitioner application";
            var firstName = WebUtility.HtmlEncode(accountRequest.Name.FirstName);
            var lastName = WebUtility.HtmlEncode(accountRequest.Name.LastName);
            var reason = WebUtility.HtmlEncode(request.AdminNote);
            var body = $"""
                <h2>Hello Dr. {firstName} {lastName}</h2>
                <p>Unfortunately your practitioner account request has not been approved.</p>
                <p><strong>Reason:</strong> {reason}</p>
                <p>If you believe this is a mistake or would like to reapply, please contact support.</p>
                """;

            await _emailService.SendEmailAsync(accountRequest.Email, subject, body, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
