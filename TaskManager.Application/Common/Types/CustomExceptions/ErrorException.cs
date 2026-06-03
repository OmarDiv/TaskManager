namespace TaskManager.Application.Common.Types.CustomExceptions;

public class ErrorException(ResultMessage error) : Exception(error.MessageCode)
{
    public ResultMessage Error { get; } = error;
}