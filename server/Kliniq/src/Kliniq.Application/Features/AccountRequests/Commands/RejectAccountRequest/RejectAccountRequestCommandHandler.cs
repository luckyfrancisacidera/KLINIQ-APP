using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using MediatR;

namespace Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest
{
    public class RejectAccountRequestCommandHandler : IRequestHandler<RejectAccountRequestCommand, bool>
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

        public async Task<bool> Handle(RejectAccountRequestCommand request, CancellationToken cancellationToken)
        {
            var accountRequest = await _repository.GetByIdAsync(request.AccountRequestId, cancellationToken);

            if(accountRequest is null )
                throw new InvalidOperationException("Account request nof found.");

            accountRequest.Reject(request.AdminNote);

            var subject = "Update on your Kliniq Practitioner Account Request";
            var body = $"""
                <h2>Hello Dr. {accountRequest.Name.FirstName} {accountRequest.Name.LastName}</h2>
                <p>Unfortunately your practitioner account request has not been approved.</p>
                <p><strong>Reason:</strong> {request.AdminNote}</p>
                <p>If you believe this is a mistake or would like to reapply, please contact support.</p>
                """;

            await _emailService.SendEmailAsync(accountRequest.Email, subject, body, cancellationToken);

            return true;
        }
    }
}
