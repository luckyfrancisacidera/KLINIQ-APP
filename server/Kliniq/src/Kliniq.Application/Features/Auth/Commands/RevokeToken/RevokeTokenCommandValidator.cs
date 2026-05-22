using FluentValidation;

namespace Kliniq.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommandValidator :AbstractValidator<RevokeTokenCommand>    
    {
        public RevokeTokenCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("User ID must be a valid GUID.");
        }
    }
}
