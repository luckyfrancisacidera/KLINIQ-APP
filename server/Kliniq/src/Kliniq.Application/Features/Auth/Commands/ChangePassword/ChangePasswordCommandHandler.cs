using Kliniq.Application.Common.Interfaces;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IAuthService _authService;
    public ChangePasswordCommandHandler(IAuthService authService) => _authService = authService;
    public Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        => _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword, cancellationToken);
}
