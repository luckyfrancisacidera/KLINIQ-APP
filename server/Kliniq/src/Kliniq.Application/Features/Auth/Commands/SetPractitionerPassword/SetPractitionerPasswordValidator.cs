using FluentValidation;

namespace Kliniq.Application.Features.Auth.Commands.SetPractitionerPassword
{
    public class SetPractitionerPasswordValidator : AbstractValidator<SetPractitionerPasswordCommand>
    {
        public SetPractitionerPasswordValidator()
        {
            RuleFor(x => x.InvitationToken)
              .NotEmpty().WithMessage("Invitation token is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[!@#$%^&*(),.?\":{}|<>]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.Password).WithMessage("Confirm password must match the password.");

            RuleFor(x => x.ClinicId)
                .NotEmpty().WithMessage("Clinic ID is required.");
        }
    }
}
