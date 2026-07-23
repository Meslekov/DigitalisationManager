namespace DigitalisationManager.Tests.Integration.PublicDemo;

using DigitalisationManager.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

internal sealed class PublicDemoWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private const string ConnectionStringVariable =
        "ConnectionStrings__DefaultConnection";

    private const string TestConnectionString =
        "Server=(localdb)\\mssqllocaldb;" +
        "Database=DigitalisationManagerIntegrationTests;" +
        "Trusted_Connection=True;" +
        "TrustServerCertificate=True";

    private readonly bool publicDemoEnabled;

    public string StorageRoot { get; }

    public PublicDemoWebApplicationFactory(
        bool publicDemoEnabled = true)
    {
        this.publicDemoEnabled = publicDemoEnabled;

        StorageRoot = Path.Combine(
             Path.GetTempPath(),
             "DigitalisationManager.Tests",
             Guid.NewGuid().ToString("N"));
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                Dictionary<string, string?> settings = new()
                {
                    ["PublicDemo:Enabled"] =
                        publicDemoEnabled.ToString(),

                    ["FileStorage:RootFolder"] =
                        StorageRoot,

                    ["FileStorage:MaxTiffUploadSizeBytes"] =
                        "1048576"
                };

                configuration.AddInMemoryCollection(settings);
            });

        builder.ConfigureTestServices(services =>
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        TestAuthHandler.SchemeName;

                    options.DefaultChallengeScheme =
                        TestAuthHandler.SchemeName;

                    options.DefaultForbidScheme =
                        TestAuthHandler.SchemeName;
                })
                .AddScheme<
                    AuthenticationSchemeOptions,
                    TestAuthHandler>(
                        TestAuthHandler.SchemeName,
                        _ => { });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        string? originalConnectionString =
            Environment.GetEnvironmentVariable(
                ConnectionStringVariable);

        Environment.SetEnvironmentVariable(
            ConnectionStringVariable,
            TestConnectionString);

        try
        {
            return base.CreateHost(builder);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                ConnectionStringVariable,
                originalConnectionString);
        }
    }

    public HttpClient CreateHttpsClient()
    {
        return CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });
    }
}