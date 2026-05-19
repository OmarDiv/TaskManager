using MediatR;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Auth.Responses;
using TaskManager.Application.Feature.Auth.Services;

namespace TaskManager.Application.Feature.Auth.Commands.GetRefreshToken
{
    public record GetRefrshToken(string Token, string RefreshToken) : IRequest<Result<AuthResponse>>;
    public class GetRefrshTokenHandler(IAuthService authService) : IRequestHandler<GetRefrshToken, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(GetRefrshToken request, CancellationToken cancellationToken)
        {
            return await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        }
    }
}
