using FluentValidation;

namespace Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest
{
    public class ApproveAccountRequestCommandValidator : AbstractValidator<ApproveAccountRequestCommand>
    {
        public ApproveAccountRequestCommandValidator()
        {
            RuleFor(x => x.AccountRequestId)
                .NotEmpty().WithMessage("Account request ID is required.");

            RuleFor(x => x.AdminNote)
                .MaximumLength(500).WithMessage("Admin not must not exceed 500 characterds")
                .When(x => !string.IsNullOrEmpty(x.AdminNote));
        }
    }
}
