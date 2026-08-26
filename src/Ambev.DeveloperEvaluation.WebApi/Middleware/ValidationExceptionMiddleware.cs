using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteAsync(context, StatusCodes.Status404NotFound, "Resource not found", ex.Message);
            }
            catch (DomainException ex)
            {
                await WriteAsync(context, StatusCodes.Status400BadRequest, "Business rule violation", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await WriteAsync(context, StatusCodes.Status409Conflict, "Conflict", ex.Message);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation Failed",
                Errors = exception.Errors
                    .Select(error => (ValidationErrorDetail)error)
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }

        private static Task WriteAsync(HttpContext context, int statusCode, string message, string detail)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = new[] { new ValidationErrorDetail { Error = message, Detail = detail } }
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }
    }
}
