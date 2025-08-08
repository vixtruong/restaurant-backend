using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Restaurant.Shared.Core;

namespace Restaurant.Shared.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, "AppException caught");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, AppException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.Code;

            var response = new
            {
                code = ex.Code,
                type = ex.ErrorType,
                message = ex.Message
            };

            return context.Response.WriteAsync(response.ToString());
        }
    }
}
