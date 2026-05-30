using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Types;

namespace TaskManager.Application.Common.Extensions
{
    public static class ValidatorsExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> NotEmptyWithMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, ResultMessage resultMessage)
        {
            return ruleBuilder.NotNull().WithMessage(resultMessage).NotEmpty().WithMessage(resultMessage);
        }

        public static IRuleBuilderOptions<T, IEnumerable<LocalizationDto>> MustContainArabicLocalization<T>(this IRuleBuilder<T, IEnumerable<LocalizationDto>> ruleBuilder, ResultMessage resultMessage)
        {
            return ruleBuilder.Must(names => names != null && names.Any(n => n.Key == "ar" && !string.IsNullOrWhiteSpace(n.Value)))
                              .WithMessage(resultMessage);
        }
    }
}
