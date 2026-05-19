using MediatR;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Auth.Services;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Feature.Users.Commands.RegisterUser
{
    public record RegisterUser(string Email, string Password, string FirstName, string LastName, Gender Gender) : IRequest<Result>;
    public class RegisterUserHandler(IAuthService _authService) : IRequestHandler<RegisterUser, Result>
    {
        public async Task<Result> Handle(RegisterUser request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterUserAsync(request, cancellationToken);
        }
    }
}
