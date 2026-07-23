namespace DigitalisationManager.Tests.Integration.PublicDemo;

using System.Net;
using System.Net.Http.Headers;

[TestFixture]
[NonParallelizable]
public class PublicDemoIntegrationTests
{
    [Test]
    public async Task LoginPage_IsAvailable_WhenPublicDemoEnabled()
    {
        using PublicDemoWebApplicationFactory factory =
            new(publicDemoEnabled: true);

        using HttpClient client =
             factory.CreateHttpsClient();

        using HttpResponseMessage response =
            await client.GetAsync("/Identity/Account/Login");

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.OK));
    }


    [Test]
    public async Task RegisterPost_IsBlocked_WhenPublicDemoEnabled()
    {
        using PublicDemoWebApplicationFactory factory =
            new(publicDemoEnabled: true);

        using HttpClient client =
             factory.CreateHttpsClient();

        using FormUrlEncodedContent content = new(
            new Dictionary<string, string>
            {
                ["Input.FirstName"] = "Integration",
                ["Input.LastName"] = "Test",
                ["Input.Email"] = "integration@test.local",
                ["Input.Password"] = "test",
                ["Input.ConfirmPassword"] = "test"
            });

        using HttpResponseMessage response =
            await client.PostAsync(
                "/Identity/Account/Register",
                content);

        string responseText =
            await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.Forbidden));

            Assert.That(
                responseText,
                Does.Contain(
                    "unavailable in the public demonstration"));
        });
    }


    [Test]
    public async Task RegisterPost_IsNotBlocked_WhenPublicDemoDisabled()
    {
        using PublicDemoWebApplicationFactory factory =
            new(publicDemoEnabled: false);

        using HttpClient client =
            factory.CreateHttpsClient();

        using FormUrlEncodedContent content = new(
            new Dictionary<string, string>
            {
                ["Input.FirstName"] = "Integration",
                ["Input.LastName"] = "Test",
                ["Input.Email"] = "integration@test.local",
                ["Input.Password"] = "test",
                ["Input.ConfirmPassword"] = "test"
            });

        using HttpResponseMessage response =
            await client.PostAsync(
                "/Identity/Account/Register",
                content);

        Assert.That(
            response.StatusCode,
            Is.Not.EqualTo(HttpStatusCode.Forbidden));
    }


    [Test]
    public async Task UploadPost_IsBlocked_BeforeFileStorage()
    {
        using PublicDemoWebApplicationFactory factory =
            new(publicDemoEnabled: true);

        using HttpClient client =
            factory.CreateHttpsClient();

        using MultipartFormDataContent form = new();

        using ByteArrayContent fileContent = new(
            [0x49, 0x49, 0x2A, 0x00]);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("image/tiff");

        form.Add(
            new StringContent("1"),
            "itemId");

        form.Add(
            fileContent,
            "files",
            "integration-test.tif");

        using HttpResponseMessage response =
            await client.PostAsync(
                "/Manager/DigitalFiles/Upload",
                form);

        Assert.Multiple(() =>
        {
            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.Forbidden));

            Assert.That(
                Directory.Exists(factory.StorageRoot),
                Is.False);
        });
    }


    [TestCase(
    "POST",
    "/Admin/Users/EditRole")]
    [TestCase(
    "POST",
    "/Manager/DigitalFiles/Delete")]
    [TestCase(
    "POST",
    "/Manager/DigitalFiles/SetDownloadAllowed")]
    [TestCase(
    "POST",
    "/Manager/Items/DeleteConfirmed")]
    [TestCase(
    "POST",
    "/Manager/Funds/DeleteConfirmed")]
    [TestCase(
    "GET",
    "/Manager/DigitalFiles/DownloadOriginal/1")]
    public async Task RestrictedMvcOperation_IsBlocked(
    string method,
    string path)
    {
        using PublicDemoWebApplicationFactory factory =
            new(publicDemoEnabled: true);

        using HttpClient client =
             factory.CreateHttpsClient();

        using HttpRequestMessage request = new(
            new HttpMethod(method),
            path);

        if (method == "POST")
        {
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>());
        }

        using HttpResponseMessage response =
            await client.SendAsync(request);

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.Forbidden));
    }


    [TestCase(
    "GET",
    "/Identity/Account/Manage/Index")]
    [TestCase(
    "POST",
    "/Identity/Account/Manage/Index")]
    [TestCase(
    "GET",
    "/Identity/Account/Manage/ChangePassword")]
    [TestCase(
    "POST",
    "/Identity/Account/Manage/ChangePassword")]
    [TestCase(
    "GET",
    "/Identity/Account/Manage/DeletePersonalData")]
    [TestCase(
    "POST",
    "/Identity/Account/Manage/DeletePersonalData")]
    public async Task IdentityManageOperation_IsBlocked(
    string method,
    string path)
    {
        using PublicDemoWebApplicationFactory factory =
            new(publicDemoEnabled: true);

        using HttpClient client =
            factory.CreateHttpsClient();

        using HttpRequestMessage request = new(
            new HttpMethod(method),
            path);

        if (method == "POST")
        {
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>());
        }

        using HttpResponseMessage response =
            await client.SendAsync(request);

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.Forbidden));
    }


    [TestCase("/Identity/Account/ForgotPassword")]
    [TestCase("/Identity/Account/ResetPassword")]
    [TestCase("/Identity/Account/ResendEmailConfirmation")]
    public async Task PasswordRecoveryPost_IsBlocked(
    string path)
    {
        using PublicDemoWebApplicationFactory factory =
            new(publicDemoEnabled: true);

        using HttpClient client =
            factory.CreateHttpsClient();

        using FormUrlEncodedContent content = new(
            new Dictionary<string, string>());

        using HttpResponseMessage response =
            await client.PostAsync(path, content);

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.Forbidden));
    }
}