using DigitalisationManager.Web.PublicDemo;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Moq;

namespace DigitalisationManager.Tests.Web.PublicDemo;

[TestFixture]
public class PublicDemoBlockedTagHelperTests
{
    [Test]
    public void Process_RemovesAnchorNavigation_WhenEnabled()
    {
        PublicDemoBlockedTagHelper tagHelper =
            CreateTagHelper(enabled: true);

        TagHelperContext context = CreateContext();

        TagHelperOutput output = CreateOutput(
            tagName: "a",
            new TagHelperAttribute("demo-blocked"),
            new TagHelperAttribute(
                "href",
                "/Identity/Account/Manage/Index"));

        tagHelper.Process(context, output);

        Assert.Multiple(() =>
        {
            Assert.That(
                output.Attributes.ContainsName("demo-blocked"),
                Is.False);

            Assert.That(
                output.Attributes.ContainsName("href"),
                Is.False);

            Assert.That(
                output.Attributes["role"].Value,
                Is.EqualTo("button"));

            Assert.That(
                output.Attributes["data-bs-toggle"].Value,
                Is.EqualTo("modal"));

            Assert.That(
                output.Attributes["data-bs-target"].Value,
                Is.EqualTo("#publicDemoRestrictionModal"));
        });
    }

    [Test]
    public void Process_PreventsButtonSubmission_WhenEnabled()
    {
        PublicDemoBlockedTagHelper tagHelper =
            CreateTagHelper(enabled: true);

        TagHelperContext context = CreateContext();

        TagHelperOutput output = CreateOutput(
            tagName: "button",
            new TagHelperAttribute("demo-blocked"),
            new TagHelperAttribute("type", "submit"),
            new TagHelperAttribute("formaction", "/restricted"));

        tagHelper.Process(context, output);

        Assert.Multiple(() =>
        {
            Assert.That(
                output.Attributes["type"].Value,
                Is.EqualTo("button"));

            Assert.That(
                output.Attributes.ContainsName("formaction"),
                Is.False);

            Assert.That(
                output.Attributes["data-bs-toggle"].Value,
                Is.EqualTo("modal"));

            Assert.That(
                output.Attributes["data-bs-target"].Value,
                Is.EqualTo("#publicDemoRestrictionModal"));
        });
    }

    [Test]
    public void Process_PreservesAnchorNavigation_WhenDisabled()
    {
        PublicDemoBlockedTagHelper tagHelper =
            CreateTagHelper(enabled: false);

        TagHelperContext context = CreateContext();

        TagHelperOutput output = CreateOutput(
            tagName: "a",
            new TagHelperAttribute("demo-blocked"),
            new TagHelperAttribute(
                "href",
                "/Identity/Account/Manage/Index"));

        tagHelper.Process(context, output);

        Assert.Multiple(() =>
        {
            Assert.That(
                output.Attributes.ContainsName("demo-blocked"),
                Is.False);

            Assert.That(
                output.Attributes["href"].Value,
                Is.EqualTo("/Identity/Account/Manage/Index"));

            Assert.That(
                output.Attributes.ContainsName("data-bs-toggle"),
                Is.False);
        });
    }

    [Test]
    public void Process_PreservesButtonSubmission_WhenDisabled()
    {
        PublicDemoBlockedTagHelper tagHelper =
            CreateTagHelper(enabled: false);

        TagHelperContext context = CreateContext();

        TagHelperOutput output = CreateOutput(
            tagName: "button",
            new TagHelperAttribute("demo-blocked"),
            new TagHelperAttribute("type", "submit"),
            new TagHelperAttribute("formaction", "/normal"));

        tagHelper.Process(context, output);

        Assert.Multiple(() =>
        {
            Assert.That(
                output.Attributes["type"].Value,
                Is.EqualTo("submit"));

            Assert.That(
                output.Attributes["formaction"].Value,
                Is.EqualTo("/normal"));

            Assert.That(
                output.Attributes.ContainsName("data-bs-toggle"),
                Is.False);
        });
    }

    private static PublicDemoBlockedTagHelper CreateTagHelper(
        bool enabled)
    {
        Mock<IOptionsMonitor<PublicDemoOptions>> options = new();

        options
            .SetupGet(x => x.CurrentValue)
            .Returns(new PublicDemoOptions
            {
                Enabled = enabled
            });

        return new PublicDemoBlockedTagHelper(options.Object);
    }

    private static TagHelperContext CreateContext()
    {
        return new TagHelperContext(
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "public-demo-test");
    }

    private static TagHelperOutput CreateOutput(
        string tagName,
        params TagHelperAttribute[] attributes)
    {
        return new TagHelperOutput(
            tagName,
            new TagHelperAttributeList(attributes),
            (_, _) => Task.FromResult<TagHelperContent>(
                new DefaultTagHelperContent()));
    }
}