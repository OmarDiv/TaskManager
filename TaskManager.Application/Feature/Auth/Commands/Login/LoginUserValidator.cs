using FluentValidation;
using TaskManager.Application.Common.Const;

namespace TaskManager.Application.Feature.Auth.Commands.Login
{
    public class LoginUserValidator : AbstractValidator<LoginUser>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ResultMessage.Required)
                .EmailAddress()
                .WithMessage(ResultMessage.InvalidEmail);

            RuleFor(x => x.Password)
               .NotEmpty()
               .WithMessage(ResultMessage.Required)
               .Matches(RegexPatterns.Password)
               .WithMessage(ResultMessage.PasswordComplexity);
        }
    }
}
