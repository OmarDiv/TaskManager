using TaskManager.Application.Common.Types.CustomExceptions;
namespace TaskManager.Application.Common.Types;

public sealed record ResultMessage(string MessageCode, object[]? Args = null)
{
    public ResultMessage WithArgs(params object[] args)
        => this with { Args = args };

    public static readonly ResultMessage None = new(string.Empty);
    public static readonly ResultMessage OperationSuccessfully = new(nameof(OperationSuccessfully));
    public static readonly ResultMessage GenralError = new(nameof(GenralError));
    public static readonly ResultMessage SavedError = new(nameof(SavedError));
    public static readonly ResultMessage YouCantDeleteThisElementBecauseHaveRelatedData =
        new(nameof(YouCantDeleteThisElementBecauseHaveRelatedData));

    public static readonly ResultMessage ProjectNotFound = new(nameof(ProjectNotFound));
    public static readonly ResultMessage ProjectUnauthorizedAccess = new(nameof(ProjectUnauthorizedAccess));
    public static readonly ResultMessage ProjectDuplicateName = new(nameof(ProjectDuplicateName));

    public static readonly ResultMessage TaskNotFound = new(nameof(TaskNotFound));
    public static readonly ResultMessage TaskUnauthorizedAccess = new(nameof(TaskUnauthorizedAccess));

    public static readonly ResultMessage InvalidCredentials = new(nameof(InvalidCredentials));
    public static readonly ResultMessage DuplicatedEmail = new(nameof(DuplicatedEmail));
    public static readonly ResultMessage UserNotFound = new(nameof(UserNotFound));
    public static readonly ResultMessage InvalidUserOrRefreshToken = new(nameof(InvalidUserOrRefreshToken));
    public static readonly ResultMessage FailedToUpdateUser = new(nameof(FailedToUpdateUser));
    public static readonly ResultMessage InvalidEmail = new(nameof(InvalidEmail));
    public static readonly ResultMessage PasswordComplexity = new(nameof(PasswordComplexity));

    public static readonly ResultMessage RoleNotFound = new(nameof(RoleNotFound));
    public static readonly ResultMessage RoleAlreadyExists = new(nameof(RoleAlreadyExists));
    public static readonly ResultMessage InvalidPermissions = new(nameof(InvalidPermissions));

    public static readonly ResultMessage Required = new(nameof(Required));
    public static readonly ResultMessage MaxLength = new(nameof(MaxLength));
    public static readonly ResultMessage GreaterThan = new(nameof(GreaterThan));
    public static readonly ResultMessage ArabicLanguageRequired = new(nameof(ArabicLanguageRequired));
    public static implicit operator string(ResultMessage message) => message.MessageCode;
    public static implicit operator ResultMessage(string message) => new(message);
    public static implicit operator Exception(ResultMessage error) => new ErrorException(error);
}