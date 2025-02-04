using PensionSystem.Application.DTOS;
using PensionSystem.Application.Extensions;

namespace PensionSystem.Presentation.Configurations
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unhandled exception occurred.");

                context.Response.ContentType = "application/json";

                var responseModel = GlobalResponse<string>.Fail(exception.Message);

                context.Response.StatusCode = exception switch
                {
                    NotFoundException or KeyNotFoundException => StatusCodes.Status404NotFound,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized, 
                    _ => StatusCodes.Status500InternalServerError
                };

                responseModel.Status = context.Response.StatusCode;
                responseModel.StatusText = GetStatusText(context.Response.StatusCode);

                await context.Response.WriteAsJsonAsync(responseModel);
            }
        }

        private string GetStatusText(int statusCode) => statusCode switch
        {
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status500InternalServerError => "Internal Server Error",
            _ => "Error"
        };
    }
}
