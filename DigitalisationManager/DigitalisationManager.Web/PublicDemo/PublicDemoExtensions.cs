namespace DigitalisationManager.Web.PublicDemo
{
    public static class PublicDemoExtensions
    {
        public static IServiceCollection AddPublicDemo(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<PublicDemoOptions>()
                .Bind(configuration.GetSection(
                    PublicDemoOptions.SectionName))
                .ValidateOnStart();

            return services;
                
        }

        public static IApplicationBuilder UsePublicDemoGuard(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<PublicDemoGuardMiddleware>();
        }
    }
}
