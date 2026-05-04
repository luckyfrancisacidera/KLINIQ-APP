using FluentValidation;

namespace Kliniq.Application.Features.AccountRequests.Commands.RejectAccountRequest
{
    public class RejectAccountRequestValidator : AbstractValidator<RejectAccountRequestCommand>
    {
        public RejectAccountRequestValidator()
        {
            RuleFor(x => x.AccountRequestId)
                .NotEmpty().WithMessage("Account request ID is required.");

             RuleFor(x => x.AdminNote)
                .NotEmpty().WithMessage("Admin note is required when rejecting a requst.")
                .MaximumLength(500).WithMessage("Admin note must not exceed 500 characters.");
        }
    }
}
