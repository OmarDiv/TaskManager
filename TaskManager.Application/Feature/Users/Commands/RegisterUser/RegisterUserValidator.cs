using FluentValidation;
using TaskManager.Application.Common.Const;
using TaskManager.Application.Common.Types;

namespace TaskManager.Application.Feature.Users.Commands.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterUser>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ResultMessage.Required)
                .EmailAddress().WithMessage(ResultMessage.InvalidEmail);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ResultMessage.Required)
                .Matches(RegexPatterns.Password)
                .WithMessage(ResultMessage.PasswordComplexity);

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(ResultMessage.Required);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(ResultMessage.Required);
                
            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage(ResultMessage.Required);
        }
    }
}
