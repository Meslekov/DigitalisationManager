using static DigitalisationManager.GCommon.ApplicationConstants;

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalisationManager.Tests.Integration.PublicDemo;

internal sealed class TestAuthHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "IntegrationTest";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        Claim[] claims =
        [
            new(
                ClaimTypes.NameIdentifier,
                "integration-test-user"),

            new(
                ClaimTypes.Name,
                "integration@test.local"),

            new(
                ClaimTypes.Role,
                RoleNames.Administrator),

            new(
                ClaimTypes.Role,
                RoleNames.Manager),

            new(
                ClaimTypes.Role,
                RoleNames.User)
        ];

        ClaimsIdentity identity = new(
            claims,
            SchemeName);

        ClaimsPrincipal principal = new(identity);

        AuthenticationTicket ticket = new(
            principal,
            SchemeName);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}