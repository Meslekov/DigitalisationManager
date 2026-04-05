namespace DigitalisationManager.Web.Extensions
{
    using DigitalisationManager.Web.Middleware;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}