using DigitalisationManager.Web.PublicDemo;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DigitalisationManager.Tests.Web.PublicDemo;

[TestFixture]
public class PublicDemoGuardMiddlewareTests
{
    [TestCase("Admin", "Users", "EditRole", "POST")]
    [TestCase("Manager", "DigitalFiles", "Upload", "POST")]
    [TestCase("Manager", "DigitalFiles", "Delete", "POST")]
    [TestCase("Manager", "DigitalFiles", "SetDownloadAllowed", "POST")]
    [TestCase("Manager", "DigitalFiles", "DownloadOriginal", "GET")]
    [TestCase("Manager", "Items", "DeleteConfirmed", "POST")]
    [TestCase("Manager", "Funds", "DeleteConfirmed", "POST")]
    public async Task InvokeAsync_BlocksConfiguredMvcOperation(
        string area,
        string controller,
        string action,
        string method)
    {
        DefaultHttpContext context =
            CreateMvcContext(area, controller, action, method);

        bool nextCalled = false;

        PublicDemoGuardMiddleware middleware = CreateMiddleware(
            enabled: true,
            next: _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        await middleware.InvokeAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo(403));
            Assert.That(nextCalled, Is.False);
        });
    }

    [TestCase("/Account/Register", "POST")]
    [TestCase("/Account/Manage", "GET")]
    [TestCase("/Account/Manage", "POST")]
    [TestCase("/Account/Manage/Index", "GET")]
    [TestCase("/Account/Manage/Index", "POST")]
    [TestCase("/Account/Manage/ChangePassword", "GET")]
    [TestCase("/Account/Manage/ChangePassword", "POST")]
    [TestCase("/Account/Manage/DeletePersonalData", "GET")]
    [TestCase("/Account/Manage/DeletePersonalData", "POST")]
    [TestCase("/Account/ForgotPassword", "POST")]
    [TestCase("/Account/ResetPassword", "POST")]
    [TestCase("/Account/ResendEmailConfirmation", "POST")]
    public async Task InvokeAsync_BlocksConfiguredRazorPage(
        string page,
        string method)
    {
        DefaultHttpContext context =
            CreateRazorPageContext("Identity", page, method);

        bool nextCalled = false;

        PublicDemoGuardMiddleware middleware = CreateMiddleware(
            enabled: true,
            next: _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        await middleware.InvokeAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo(403));
            Assert.That(nextCalled, Is.False);
        });
    }

    [Test]
    public async Task InvokeAsync_CallsNext_WhenPublicDemoIsDisabled()
    {
        DefaultHttpContext context = CreateMvcContext(
            "Manager",
            "DigitalFiles",
            "Upload",
            "POST");

        bool nextCalled = false;

        PublicDemoGuardMiddleware middleware = CreateMiddleware(
            enabled: false,
            next: _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        await middleware.InvokeAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(nextCalled, Is.True);
            Assert.That(context.Response.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task InvokeAsync_MatchesRoutesCaseInsensitively()
    {
        DefaultHttpContext context = CreateMvcContext(
            "manager",
            "digitalfiles",
            "upload",
            "post");

        bool nextCalled = false;

        PublicDemoGuardMiddleware middleware = CreateMiddleware(
            enabled: true,
            next: _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        await middleware.InvokeAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo(403));
            Assert.That(nextCalled, Is.False);
        });
    }

    [TestCase("Manager", "DigitalFiles", "Preview", "GET")]
    [TestCase("Manager", "DigitalFiles", "DownloadPreview", "GET")]
    [TestCase("Manager", "Items", "Edit", "POST")]
    [TestCase("Manager", "Funds", "Create", "POST")]
    public async Task InvokeAsync_AllowsUnrelatedMvcOperation(
        string area,
        string controller,
        string action,
        string method)
    {
        DefaultHttpContext context =
            CreateMvcContext(area, controller, action, method);

        bool nextCalled = false;

        PublicDemoGuardMiddleware middleware = CreateMiddleware(
            enabled: true,
            next: _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        await middleware.InvokeAsync(context);

        Assert.That(nextCalled, Is.True);
    }

    [TestCase("/Account/Login", "GET")]
    [TestCase("/Account/Login", "POST")]
    [TestCase("/Account/Logout", "POST")]
    [TestCase("/Account/Management", "GET")]
    public async Task InvokeAsync_AllowsUnrelatedRazorPage(
        string page,
        string method)
    {
        DefaultHttpContext context =
            CreateRazorPageContext("Identity", page, method);

        bool nextCalled = false;

        PublicDemoGuardMiddleware middleware = CreateMiddleware(
            enabled: true,
            next: _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        await middleware.InvokeAsync(context);

        Assert.That(nextCalled, Is.True);
    }

    [Test]
    public async Task InvokeAsync_WritesExplanationForBlockedRequest()
    {
        DefaultHttpContext context = CreateMvcContext(
            "Admin",
            "Users",
            "EditRole",
            "POST");

        PublicDemoGuardMiddleware middleware = CreateMiddleware(
            enabled: true,
            next: _ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;

        using StreamReader reader =
            new StreamReader(context.Response.Body);

        string response = await reader.ReadToEndAsync();

        Assert.That(
            response,
            Does.Contain("unavailable in the public demonstration"));
    }

    private static PublicDemoGuardMiddleware CreateMiddleware(
        bool enabled,
        RequestDelegate next)
    {
        Mock<IOptionsMonitor<PublicDemoOptions>> options = new();

        options
            .SetupGet(x => x.CurrentValue)
            .Returns(new PublicDemoOptions
            {
                Enabled = enabled
            });

        return new PublicDemoGuardMiddleware(
            next,
            options.Object,
            Mock.Of<ILogger<PublicDemoGuardMiddleware>>());
    }

    private static DefaultHttpContext CreateMvcContext(
        string area,
        string controller,
        string action,
        string method)
    {
        DefaultHttpContext context = CreateContext(method);

        context.Request.RouteValues = new RouteValueDictionary
        {
            ["area"] = area,
            ["controller"] = controller,
            ["action"] = action
        };

        return context;
    }

    private static DefaultHttpContext CreateRazorPageContext(
        string area,
        string page,
        string method)
    {
        DefaultHttpContext context = CreateContext(method);

        context.Request.RouteValues = new RouteValueDictionary
        {
            ["area"] = area,
            ["page"] = page
        };

        return context;
    }

    private static DefaultHttpContext CreateContext(string method)
    {
        DefaultHttpContext context = new();

        context.Request.Method = method;
        context.Response.Body = new MemoryStream();

        return context;
    }
}