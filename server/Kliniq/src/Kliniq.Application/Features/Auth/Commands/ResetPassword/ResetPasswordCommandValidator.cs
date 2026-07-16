using FluentValidation;

namespace Kliniq.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).Matches(@"\d").Matches("[A-Z]").Matches("[a-z]").Matches(@"[\W_]");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
    }
}
