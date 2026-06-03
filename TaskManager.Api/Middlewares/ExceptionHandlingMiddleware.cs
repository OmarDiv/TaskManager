using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Types.CustomExceptions;

namespace TaskManager.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, IStringLocalizer stringLocalizer, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _stringLocalizer = stringLocalizer;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogDebug("Processing request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await _next(context);
                _logger.LogDebug("Request completed successfully: {Method} {Path} - Status: {StatusCode}",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing request: {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                await HandleException(context, ex);
                return;
            }
        }

        private Task HandleException(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            _logger.LogDebug("Handling exception of type: {ExceptionType}", exception.GetType().Name);

            return exception switch
            {
                NotFoundException notFoundException => HandleException(response, HttpStatusCode.NotFound, notFoundException.Message, nameof(NotFoundException)),
                SaveChangesException saveChangesException => HandleSaveChangesException(response, saveChangesException),
                UnauthorizedAccessException unauthorizedAccessException => HandleException(response, HttpStatusCode.Unauthorized, _stringLocalizer[unauthorizedAccessException.Message], nameof(UnauthorizedAccessException)),
                InvalidOperationException invalidOperationException => HandleInvalidOperationException(response, invalidOperationException),
                DbUpdateException dbUpdateException => HandleDbUpdateException(response, dbUpdateException),
                ErrorException errorException => HandleErrorException(response, errorException),
                _ => HandleException(response, HttpStatusCode.InternalServerError, _stringLocalizer[ResultMessage.GenralError], "UnhandledException"),
            };
        }


        private Task HandleSaveChangesException(HttpResponse response, SaveChangesException exception)
        {
            _logger.LogWarning("SaveChangesException occurred: {Message}", exception.Message);

            CultureInfo.CurrentCulture = CultureInfo.CurrentCulture.Name == "en-US" ? new CultureInfo("ar") : CultureInfo.CurrentCulture;

            _logger.LogDebug("Culture changed for SaveChangesException handling. Current culture: {Culture}", CultureInfo.CurrentCulture.Name);

            return HandleException(response, HttpStatusCode.BadRequest, _stringLocalizer[exception.Message], "SaveChangesException");
        }

        private Task HandleException(HttpResponse response, HttpStatusCode statusCode, string message, string exceptionType)
        {
            response.StatusCode = (int)statusCode;

            _logger.LogInformation("Returning {StatusCode} response for {ExceptionType}: {Message}",
                (int)statusCode, exceptionType, message);

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            var result = JsonConvert.SerializeObject(Result.Failure(new ResultMessage(message)), serializerSettings);

            _logger.LogDebug("Serialized error response: {Response}", result);

            return response.WriteAsync(result);
        }

        private Task HandleErrorException(HttpResponse response, ErrorException errorException)
        {
            _logger.LogWarning("ErrorException occurred: {Error}", errorException.Error);

            response.StatusCode = (int)HttpStatusCode.BadRequest;

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            var result = JsonConvert.SerializeObject(Result.Failure(new ResultMessage(errorException.Error)), serializerSettings);

            _logger.LogInformation("Returning 400 response for ErrorException: {Error}", errorException.Error);
            _logger.LogDebug("Serialized error response: {Response}", result);

            return response.WriteAsync(result);
        }

        private Task HandleDbUpdateException(HttpResponse response, DbUpdateException dbUpdateException)
        {
            _logger.LogError(dbUpdateException, "DbUpdateException occurred");

            response.StatusCode = (int)HttpStatusCode.BadRequest;
            var innerException = dbUpdateException.InnerException as SqlException;

            if (innerException != null)
            {
                _logger.LogWarning("SQL Exception details - Number: {SqlErrorNumber}, Message: {SqlMessage}",
                    innerException.Number, innerException.Message);

                return innerException.Number switch
                {
                    547 => HandleForeignKeyConstraintViolation(response),
                    515 => HandleNotNullConstraintViolation(response, innerException.Message),
                    _ => HandleGenericDbError(response),
                };
            }

            _logger.LogWarning("DbUpdateException without SQL inner exception");
            return HandleGenericDbError(response);
        }

        private Task HandleForeignKeyConstraintViolation(HttpResponse response)
        {
            _logger.LogInformation("Handling foreign key constraint violation (SQL Error 547)");
            return HandleException(response, HttpStatusCode.BadRequest,
                _stringLocalizer[ResultMessage.YouCantDeleteThisElementBecauseHaveRelatedData],
                "ForeignKeyConstraintViolation");
        }

        private Task HandleNotNullConstraintViolation(HttpResponse response, string sqlMessage)
        {
            var extractedMessage = ExtractNotNullErrorMessage(sqlMessage);
            _logger.LogInformation("Handling NOT NULL constraint violation (SQL Error 515): {ExtractedMessage}", extractedMessage);
            return HandleException(response, HttpStatusCode.BadRequest, extractedMessage, "NotNullConstraintViolation");
        }

        private Task HandleGenericDbError(HttpResponse response)
        {
            _logger.LogInformation("Handling generic database error");
            return HandleException(response, HttpStatusCode.BadRequest,
                _stringLocalizer[ResultMessage.SavedError],
                "GenericDbError");
        }

        private string ExtractNotNullErrorMessage(string errorMessage)
        {
            const string nullViolationKeyword = "Cannot insert the value NULL into column";

            if (errorMessage.Contains(nullViolationKeyword))
            {
                var startIndex = errorMessage.IndexOf("column '") + 8;
                var endIndex = errorMessage.IndexOf("', table", startIndex);
                if (startIndex > 0 && endIndex > startIndex)
                {
                    var columnName = errorMessage.Substring(startIndex, endIndex - startIndex);
                    var extractedMessage = $"Cannot insert NULL into column '{columnName}'. This column does not allow null values.";

                    _logger.LogDebug("Extracted column name from NULL violation: {ColumnName}", columnName);

                    return extractedMessage;
                }
            }

            _logger.LogWarning("Could not extract column name from NULL violation message: {ErrorMessage}", errorMessage);
            return "Unknown column or table for NULL violation.";
        }

        private Task HandleInvalidOperationException(HttpResponse response, InvalidOperationException exception)
        {
            _logger.LogWarning("InvalidOperationException occurred: {Message}", exception.Message);

            // Check if it's related to severed association (Department-SchoolDepartment relationship)
            if (exception.Message.Contains("association") && exception.Message.Contains("severed"))
            {
                _logger.LogInformation("Handling severed association InvalidOperationException");
                return HandleException(response, HttpStatusCode.BadRequest,
                    _stringLocalizer[ResultMessage.YouCantDeleteThisElementBecauseHaveRelatedData],
                    "SeveredAssociationException");
            }

            // Check if it's related to required relationship
            if (exception.Message.Contains("required relationship") || exception.Message.Contains("non-nullable"))
            {
                _logger.LogInformation("Handling required relationship InvalidOperationException");
                return HandleException(response, HttpStatusCode.BadRequest,
                    _stringLocalizer[ResultMessage.YouCantDeleteThisElementBecauseHaveRelatedData],
                    "RequiredRelationshipException");
            }

            // Generic InvalidOperationException handling
            _logger.LogInformation("Handling generic InvalidOperationException");
            return HandleException(response, HttpStatusCode.BadRequest,
                _stringLocalizer[exception.Message],
                "InvalidOperationException");
        }
    }



}
