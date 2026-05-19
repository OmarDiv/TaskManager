using MediatR;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Auth.Responses;
using TaskManager.Application.Feature.Auth.Services;

namespace TaskManager.Application.Feature.Auth.Commands.Login
{
    public record LoginUser(string Email, string Password) : IRequest<Result<AuthResponse>>;
    public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginUser, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(LoginUser request, CancellationToken cancellationToken)
        {
            return await authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
        }
    }
}
