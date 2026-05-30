using MediatR;
using Microsoft.Extensions.Localization;
using TaskManager.Application.Common.Types;

namespace TaskManager.Application.Common.Behaviors;

public class LocalizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IStringLocalizer _localizer;

    public LocalizationBehavior(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var result = await next();
        // Localize the message key
        result.LocalizeMessage(_localizer);
        return result;
    }
}