using FluentValidation;

namespace Kliniq.Application.Features.AccountRequests.Commands.ApproveAccountRequest
{
    public class ApproveAccountRequestValidator : AbstractValidator<ApproveAccountRequestCommand>
    {
        public ApproveAccountRequestValidator()
        {
            RuleFor(x => x.AccountRequestId)
                .NotEmpty().WithMessage("Account request ID is required.");

            RuleFor(x => x.AdminNote)
                .MaximumLength(500).WithMessage("Admin not must not exceed 500 characterds")
                .When(x => !string.IsNullOrEmpty(x.AdminNote));
        }
    }
}
