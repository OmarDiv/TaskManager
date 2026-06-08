using Microsoft.Extensions.Localization;
using TaskManager.Application.Common.Types.CustomExceptions;

namespace TaskManager.Application.Common.Types;

public enum ResultType
{
    Success = 200,
    BadRequest = 400,
    NotFound = 404
}

public class Result
{
    private ResultMessage? _resultMessage;

    protected Result(bool isSuccess, ResultMessage resultMessage, ResultType type = ResultType.Success)
    {
        Status = isSuccess;
        _resultMessage = resultMessage;
        Message = resultMessage.MessageCode;
        Type = isSuccess ? ResultType.Success : type;
    }

    public string? Message { get; private set; }
    public bool Status { get; private set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ResultType Type { get; private set; }

    public void LocalizeMessage(IStringLocalizer localizer)
    {
        if (_resultMessage != null && _resultMessage != ResultMessage.None)
        {
            var localized = _resultMessage.Args != null && _resultMessage.Args.Length > 0
                ? localizer[_resultMessage.MessageCode, _resultMessage.Args]
                : localizer[_resultMessage.MessageCode];
            Message = localized.Value ?? _resultMessage.MessageCode;
        }
    }


    public static Result Success() => new(true, ResultMessage.OperationSuccessfully);
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result Failure(ResultMessage resultMessage) => new(false, resultMessage, ResultType.BadRequest);
    public static Result NotFound(ResultMessage resultMessage) => new(false, resultMessage, ResultType.NotFound);

    // Implicit conversion from ResultMessage to Result
    public static implicit operator Result(ResultMessage resultMessage) => resultMessage == ResultMessage.OperationSuccessfully ? Success() : Failure(resultMessage);
    // Implicit conversion from ErrorException to Result
    public static implicit operator Result(ErrorException errorException) => Failure(errorException.Message);
}

public class Result<T> : Result
{
    protected Result(bool isSuccess, ResultMessage resultMessage, T? value, ResultType type = ResultType.Success) : base(isSuccess, resultMessage, type)
    {
        Data = value;
    }

    public T? Data { get; private set; }
    public static Result<T> ValidationFailure(ResultMessage message)
    => new(false, message, default, ResultType.BadRequest);
    public static Result<T> Success(T value) => new(true, ResultMessage.OperationSuccessfully, value);
    public static new Result<T> Failure(ResultMessage resultMessage) => new(false, resultMessage, default, ResultType.BadRequest);
    public static new Result<T> NotFound(ResultMessage resultMessage) => new(false, resultMessage, default, ResultType.NotFound);

    // Implicit conversion from ResultMessage to Result<T>
    public static implicit operator Result<T>(ResultMessage resultMessage)
    {
        if (resultMessage == ResultMessage.None)
            throw new InvalidCastException("Can't create failure result from ResultMessage.None");
        return Failure(resultMessage);
    }
    // Implicit conversion from T to Result<T>
    public static implicit operator Result<T>(T value) => Success(value);
}
