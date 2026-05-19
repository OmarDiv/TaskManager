using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Authentication
{
    public interface IJwtProvider
    {
        (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        long? ValidateToken(string token);
    }
}
