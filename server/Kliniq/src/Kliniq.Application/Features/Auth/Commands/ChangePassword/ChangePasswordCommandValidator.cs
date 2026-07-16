using FluentValidation;

namespace Kliniq.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).Matches(@"\d").Matches("[A-Z]").Matches("[a-z]").Matches(@"[\W_]");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword);
        RuleFor(x => x).Must(x => x.CurrentPassword != x.NewPassword).WithMessage("The new password must be different from the current password.");
    }
}
