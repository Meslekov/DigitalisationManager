namespace DigitalisationManager.Web.Middleware
{
    using System.Net;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (FileNotFoundException ex)
            {
                logger.LogWarning(ex, "Requested file was not found. Path: {Path}", context.Request.Path);

                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.NotFound,
                    "/Error/404");
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "Unauthorized access attempt. Path: {Path}", context.Request.Path);

                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Forbidden,
                    "/Error/403");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred. Path: {Path}", context.Request.Path);

                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "/Error/500");
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string redirectPath)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.ContentType = "application/json";

                string message = statusCode switch
                {
                    HttpStatusCode.NotFound => "{\"message\":\"Resource not found.\"}",
                    HttpStatusCode.Forbidden => "{\"message\":\"Access denied.\"}",
                    _ => "{\"message\":\"An unexpected error occurred.\"}"
                };

                await context.Response.WriteAsync(message);
                return;
            }

            context.Response.Redirect(redirectPath);
        }
    }
}