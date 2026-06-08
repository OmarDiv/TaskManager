using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Infrastructure.Persistence.Data;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Application.Common.Authentication;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Auth.Responses;
using TaskManager.Application.Feature.Auth.Services;
using TaskManager.Application.Feature.Users.Commands.RegisterUser;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Context;

namespace TaskManager.Infrastructure.Services.Auth
{
    public class AuthService(
        UserManager<ApplicationUser> _userManager,
        ApplicationDbContext _context,
        IJwtProvider _jwtProvider,
        SignInManager<ApplicationUser> _signInManager,
        ILogger<AuthService> _logger
        ) : IAuthService
    {
        private static readonly int _refreshTokenExpiryDays = 14;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return ResultMessage.InvalidCredentials;

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
            {
                var (userRoles, userPermissions) = await GetUserRolesAndPermissionsAsync(user);
                (string token, int expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiration,
                });
                var isUpdated = await _userManager.UpdateAsync(user);
                if (!isUpdated.Succeeded)
                    return ResultMessage.FailedToUpdateUser;
                return Result.Success(new AuthResponse(
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    token,
                    expiresIn,
                    refreshToken,
                   refreshTokenExpiration));
            }
            return ResultMessage.InvalidCredentials;
        }

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            if (_jwtProvider.ValidateToken(token) is not long userId)
                return ResultMessage.InvalidUserOrRefreshToken;
            if (await _userManager.FindByIdAsync(userId.ToString()) is not { } user)
                return ResultMessage.InvalidUserOrRefreshToken;
            if (user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive) is not { } oldRefreshToken)
                return ResultMessage.InvalidUserOrRefreshToken;
            oldRefreshToken.RevokedOn = DateTime.UtcNow;
            var (userRoles, userPermissions) = await GetUserRolesAndPermissionsAsync(user);
            (string newToken, int expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiration,
            });
            var isUpdated = await _userManager.UpdateAsync(user);
            if (!isUpdated.Succeeded)
                return ResultMessage.FailedToUpdateUser;
            return Result.Success(new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                newToken,
                expiresIn,
                newRefreshToken,
               refreshTokenExpiration));

        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissionsAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await _context.Roles
                .Join(_context.RoleClaims,
                    role => role.Id,
                    roleClaim => roleClaim.RoleId,
                    (role, claim) => new { role, roleClaim = claim })
                .Where(x => userRoles.Contains(x.role.Name!))
                .Select(x => x.roleClaim.ClaimValue!)
                .ToListAsync();
            return (userRoles, userPermissions);
        }

        public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            if (_jwtProvider.ValidateToken(token) is not long userId)
                return ResultMessage.InvalidUserOrRefreshToken;
            if (await _userManager.FindByIdAsync(userId.ToString()) is not { } user)
                return ResultMessage.InvalidUserOrRefreshToken;
            if (user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive) is not { } oldRefreshToken)
                return ResultMessage.InvalidUserOrRefreshToken;
            oldRefreshToken.RevokedOn = DateTime.UtcNow;
            var isUpdated = await _userManager.UpdateAsync(user);
            if (!isUpdated.Succeeded)
                return ResultMessage.FailedToUpdateUser;
            return Result.Success();
        }

        public async Task<Result> RegisterUserAsync(RegisterUser request, CancellationToken cancellationToken)
        {
            if ((await _userManager.Users.AnyAsync(x => x.Email == request.Email)))
                return ResultMessage.DuplicatedEmail;

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);
                return Result.Success();
            }
            var error = result.Errors.First();
            return new ResultMessage(string.Join(",", result.Errors.Select(er=>er.Description)));
        }
    }
}
