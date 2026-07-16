using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Settings;
using Kliniq.Domain.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace Kliniq.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly AppSettings _settings;

    public ForgotPasswordCommandHandler(IAuthService authService, IEmailService emailService, IOptions<AppSettings> settings)
    {
        _authService = authService;
        _emailService = emailService;
        _settings = settings.Value;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var token = await _authService.GeneratePasswordResetTokenAsync(request.Email, cancellationToken);
        if (token is null) return Result.Success();

        var link = $"{_settings.BaseUrl.TrimEnd('/')}/reset-password?email={Uri.EscapeDataString(request.Email.Trim())}&token={Uri.EscapeDataString(token)}";
        var body = $"""
            <h2>Reset your KLINIQ password</h2>
            <p>A password reset was requested for your account.</p>
            <p><a href="{WebUtility.HtmlEncode(link)}">Reset password</a></p>
            <p>If you did not request this, you can ignore this email.</p>
            """;
        await _emailService.SendEmailAsync(request.Email.Trim(), "Reset your KLINIQ password", body, cancellationToken);
        return Result.Success();
    }
}
