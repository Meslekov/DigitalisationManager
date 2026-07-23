using Microsoft.Extensions.Options;

namespace DigitalisationManager.Web.PublicDemo
{
    public sealed class PublicDemoGuardMiddleware
    {
        private static readonly BlockedOperation[] BlockedOperations =
        [
            BlockedOperation.Mvc(
                area: "Admin",
                controller: "Users",
                action: "EditRole",
                httpMethod: HttpMethods.Post),

            BlockedOperation.Mvc(
                area: "Manager",
                controller: "DigitalFiles",
                action: "Upload",
                httpMethod: HttpMethods.Post),

            BlockedOperation.Mvc(
                area: "Manager",
                controller: "DigitalFiles",
                action: "Delete",
                httpMethod: HttpMethods.Post),

            BlockedOperation.Mvc(
                area: "Manager",
                controller: "DigitalFiles",
                action: "SetDownloadAllowed",
                httpMethod: HttpMethods.Post),

            BlockedOperation.Mvc(
                area: "Manager",
                controller: "DigitalFiles",
                action: "DownloadOriginal",
                httpMethod: HttpMethods.Get),

            BlockedOperation.Mvc(
                area: "Manager",
                controller: "Items",
                action: "DeleteConfirmed",
                httpMethod: HttpMethods.Post),

            BlockedOperation.Mvc(
                area: "Manager",
                controller: "Funds",
                action: "DeleteConfirmed",
                httpMethod: HttpMethods.Post),

            BlockedOperation.RazorPage(
                area: "Identity",
                page: "/Account/Register",
                httpMethod: HttpMethods.Post),

            BlockedOperation.RazorPage(
                area: "Identity",
                page: "/Account/ForgotPassword",
                httpMethod: HttpMethods.Post),

             BlockedOperation.RazorPage(
                area: "Identity",
                page: "/Account/ResetPassword",
                httpMethod: HttpMethods.Post),

             BlockedOperation.RazorPage(
                area: "Identity",
                page: "/Account/ResendEmailConfirmation",
                httpMethod: HttpMethods.Post),

            BlockedOperation.RazorPagePrefix(
                area: "Identity",
                pagePrefix: "/Account/Manage",
                httpMethod: HttpMethods.Get),

            BlockedOperation.RazorPagePrefix(
                area: "Identity",
                pagePrefix: "/Account/Manage",
                httpMethod: HttpMethods.Post)
        ];

        private readonly RequestDelegate next;
        private readonly IOptionsMonitor<PublicDemoOptions> options;
        private readonly ILogger<PublicDemoGuardMiddleware> logger;

        public PublicDemoGuardMiddleware(
            RequestDelegate next,
            IOptionsMonitor<PublicDemoOptions> options,
            ILogger<PublicDemoGuardMiddleware> logger)
        {
            this.next = next;
            this.options = options;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!options.CurrentValue.Enabled)
            {
                await next(context);
                return;
            }

            BlockedOperation? blockedOperation =
                FindBlockedOperation(context);

            if (blockedOperation is null)
            {
                await next(context);
                return;
            }

            logger.LogWarning(
                "Public demo blocked {Method} request. " +
                "Area: {Area}, Controller: {Controller}, " +
                "Action: {Action}, Page: {Page}, User: {User}",
                context.Request.Method,
                blockedOperation.Area,
                blockedOperation.Controller,
                blockedOperation.Action,
                blockedOperation.Page ?? blockedOperation.PagePrefix,
                context.User.Identity?.Name ?? "Anonymous");

            context.Response.StatusCode =
                StatusCodes.Status403Forbidden;

            context.Response.ContentType =
                "text/plain; charset=utf-8";

            await context.Response.WriteAsync(
                "This operation is unavailable in the public demonstration.");
        }

        private static BlockedOperation? FindBlockedOperation(
            HttpContext context)
        {
            string area = GetRouteValue(context, "area");
            string controller = GetRouteValue(context, "controller");
            string action = GetRouteValue(context, "action");
            string page = GetRouteValue(context, "page");

            return BlockedOperations.FirstOrDefault(operation =>
            {
                bool areaMatches = string.Equals(
                    operation.Area,
                    area,
                    StringComparison.OrdinalIgnoreCase);

                bool methodMatches = string.Equals(
                    operation.HttpMethod,
                    context.Request.Method,
                    StringComparison.OrdinalIgnoreCase);

                if (!areaMatches || !methodMatches)
                {
                    return false;
                }

                if (operation.Page is not null)
                {
                    return string.Equals(
                        operation.Page,
                        page,
                        StringComparison.OrdinalIgnoreCase);
                }

                if (operation.PagePrefix is not null)
                {
                    bool exactPrefixMatch = string.Equals(
                        operation.PagePrefix,
                        page,
                        StringComparison.OrdinalIgnoreCase);

                    bool childPageMatch = page.StartsWith(
                        operation.PagePrefix + "/",
                        StringComparison.OrdinalIgnoreCase);

                    return exactPrefixMatch || childPageMatch;
                }

                return string.Equals(
                           operation.Controller,
                           controller,
                           StringComparison.OrdinalIgnoreCase)
                       &&
                       string.Equals(
                           operation.Action,
                           action,
                           StringComparison.OrdinalIgnoreCase);
            });
        }

        private static string GetRouteValue(
            HttpContext context,
            string name)
        {
            return context.Request.RouteValues[name]?.ToString()
                   ?? string.Empty;
        }
    }
}