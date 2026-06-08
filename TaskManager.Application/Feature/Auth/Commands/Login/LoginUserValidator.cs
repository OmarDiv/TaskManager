using FluentValidation;
using TaskManager.Application.Common.Const;
using TaskManager.Application.Common.Extensions;

namespace TaskManager.Application.Feature.Auth.Commands.Login
{
    public class LoginUserValidator : AbstractValidator<LoginUser>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmptyWithMessage(ResultMessage.Required)
                .EmailAddress()
                .WithMessage(ResultMessage.InvalidEmail);

            RuleFor(x => x.Password)
               .NotEmptyWithMessage(ResultMessage.Required)
               .Matches(RegexPatterns.Password)
               .WithMessage(ResultMessage.PasswordComplexity);
        }
    }
}
