using FluentValidation;

namespace TaskManager.Application.Feature.Auth.Commands.RevokeRefreshToken
{
    public class RevokeRereshTokenValidator : AbstractValidator<RevokeRefreshToken>
    {
        public RevokeRereshTokenValidator()
        {
            RuleFor(x => x.Token)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.RefreshToken)
                .NotNull()
                .NotEmpty();
        }
    }
}
