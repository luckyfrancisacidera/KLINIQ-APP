using FluentValidation;

namespace Kliniq.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenValidator :AbstractValidator<RevokeTokenCommand>    
    {
        public RevokeTokenValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("User ID must be a valid GUID.");
        }
    }
}
