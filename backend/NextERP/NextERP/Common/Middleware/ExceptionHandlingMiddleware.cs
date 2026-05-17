using System.Net;
using System.Text.Json;
using NextERP.Common.DTOs;
using NextERP.Common.Exceptions;

namespace NextERP.Common.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse<object>
            {
                Success = false,
                Timestamp = DateTime.UtcNow,
                RequestId = context.TraceIdentifier
            };

            switch (exception)
            {
                case ApiException apiEx:
                    context.Response.StatusCode = apiEx.StatusCode;
                    response.StatusCode = apiEx.StatusCode;
                    response.Message = apiEx.Message;
                    response.Errors = apiEx.Errors ?? new List<string>();
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Unauthorized access";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    
                    if (_env.IsDevelopment())
                    {
                        var message = exception.Message;
                        // Basic sanitization: remove anything that looks like a JWT or a long hex/base64 string
                        // This is a safety measure requested by the user
                        response.Message = SanitizeMessage(message);
                        response.Errors.Add("Internal Server Error occurred. See logs for details.");
                    }
                    else
                    {
                        response.Message = "An internal server error occurred.";
                    }
                    break;
            }

            var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(result);
        }

        private string SanitizeMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return message;

            // Simple regex/logic to mask potential tokens or keys (very long strings with no spaces)
            var words = message.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 50) // Likely a token, key, or long hash
                {
                    words[i] = "[REDACTED]";
                }
            }
            return string.Join(" ", words);
        }
    }
}
