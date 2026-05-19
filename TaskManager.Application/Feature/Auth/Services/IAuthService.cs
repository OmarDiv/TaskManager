using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Auth.Responses;
using TaskManager.Application.Feature.Users.Commands.RegisterUser;
namespace TaskManager.Application.Feature.Auth.Services
{
    public interface IAuthService
    {

        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken);
        Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken);
        Task<Result> RegisterUserAsync(RegisterUser request, CancellationToken cancellationToken);
    }
}