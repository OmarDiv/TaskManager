using FluentValidation;

namespace TaskManager.Application.Feature.Auth.Commands.GetRefreshToken
{
    public class GetRefrshTokenValidator : AbstractValidator<GetRefrshToken>
    {
        public GetRefrshTokenValidator()
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
