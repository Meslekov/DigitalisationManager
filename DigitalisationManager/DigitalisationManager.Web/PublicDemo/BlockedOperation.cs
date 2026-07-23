namespace DigitalisationManager.Web.PublicDemo
{
    public sealed record BlockedOperation(
        string Area,
        string HttpMethod,
        string? Controller = null,
        string? Action = null,
        string? Page = null,
        string? PagePrefix = null)
    {
        public static BlockedOperation Mvc(
            string area,
            string controller,
            string action,
            string httpMethod)
        {
            return new BlockedOperation(
                Area: area,
                HttpMethod: httpMethod,
                Controller: controller,
                Action: action);
        }

        public static BlockedOperation RazorPage(
            string area,
            string page,
            string httpMethod)
        {
            return new BlockedOperation(
                Area: area,
                HttpMethod: httpMethod,
                Page: page);
        }

        public static BlockedOperation RazorPagePrefix(
            string area,
            string pagePrefix,
            string httpMethod)
        {
            return new BlockedOperation(
                Area: area,
                HttpMethod: httpMethod,
                PagePrefix: pagePrefix);
        }
    }
}