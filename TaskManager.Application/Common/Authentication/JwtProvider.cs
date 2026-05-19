using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _options = options.Value;

        public (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            Claim[] claims = [
                new( JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new( JwtRegisteredClaimNames.Email, user.Email!),
                new( JwtRegisteredClaimNames.GivenName, user.FirstName),
                new( JwtRegisteredClaimNames.FamilyName, user.LastName),
                new( JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
                new(nameof(permissions),JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)
                ];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var singningCreadeintial = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var expiresIn = _options.ExpirationInMinutes;

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
               audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: singningCreadeintial

               );
            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn * 60);

        }

        public long? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmeticSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = symmeticSecurityKey,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken securityToken);
                var jwtToken = (JwtSecurityToken)securityToken;
                var sub = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                return long.Parse(sub);
            }
            catch
            {
                return null;
            }
        }
    }
}
