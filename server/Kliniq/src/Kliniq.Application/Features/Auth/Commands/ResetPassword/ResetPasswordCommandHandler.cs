using Kliniq.Application.Common.Interfaces;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IAuthService _authService;
    public ResetPasswordCommandHandler(IAuthService authService) => _authService = authService;
    public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        => _authService.ResetPasswordAsync(request.Email, request.Token, request.Password, cancellationToken);
}
