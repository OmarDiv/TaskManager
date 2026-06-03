using Microsoft.Extensions.Localization;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result  // ← Add this constraint
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IStringLocalizer _localizer;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        IStringLocalizer localizer)
    {
        _validators = validators;
        _localizer = localizer;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var localizedErrors = failures.Select(f =>
            {
                var messageTemplate = _localizer[f.ErrorMessage].Value;
                var translatedPropertyName = _localizer[f.PropertyName].Value;
                
                var finalMessage = messageTemplate.Replace("{PropertyName}", translatedPropertyName);
                
                if (f.FormattedMessagePlaceholderValues != null)
                {
                    foreach (var placeholder in f.FormattedMessagePlaceholderValues)
                    {
                        if (placeholder.Key != "PropertyName")
                        {
                            finalMessage = finalMessage.Replace($"{{{placeholder.Key}}}", placeholder.Value?.ToString());
                        }
                    }
                }
                
                return finalMessage;
            }).Distinct();
            
            var message = string.Join(" | ", localizedErrors);

            var resultMessage = new ResultMessage(message);
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var method = typeof(Result<>).MakeGenericType(resultType).GetMethod("Failure", new[] { typeof(ResultMessage) });
                return (TResponse)method.Invoke(null, new object[] { resultMessage })!;
            }
            
            return (TResponse)(object)Result.Failure(resultMessage);
        }

        return await next();
    }
}
