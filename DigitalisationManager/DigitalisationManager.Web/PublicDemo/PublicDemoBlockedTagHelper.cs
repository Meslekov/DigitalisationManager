using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace DigitalisationManager.Web.PublicDemo
{
    [HtmlTargetElement("button", Attributes = AttributeName)]
    [HtmlTargetElement("a", Attributes = AttributeName)]
    public sealed class PublicDemoBlockedTagHelper : TagHelper
    {
        private const string AttributeName = "demo-blocked";

        private readonly IOptionsMonitor<PublicDemoOptions> options;

        public PublicDemoBlockedTagHelper(
            IOptionsMonitor<PublicDemoOptions> options)
        {
            this.options = options;
        }

        // Run after built-in anchor/form Tag Helpers.
        public override int Order => 1000;

        public override void Process(
            TagHelperContext context,
            TagHelperOutput output)
        {
            output.Attributes.RemoveAll(AttributeName);

            if (!options.CurrentValue.Enabled)
            {
                return;
            }

            output.Attributes.SetAttribute(
                "data-bs-toggle",
                "modal");

            output.Attributes.SetAttribute(
                "data-bs-target",
                "#publicDemoRestrictionModal");

            output.Attributes.SetAttribute(
                "aria-label",
                "Unavailable in the public demo");

            if (string.Equals(
                output.TagName,
                "button",
                StringComparison.OrdinalIgnoreCase))
            {
                // Prevent the form from being submitted.
                output.Attributes.SetAttribute("type", "button");
                output.Attributes.RemoveAll("formaction");
            }

            if (string.Equals(
                output.TagName,
                "a",
                StringComparison.OrdinalIgnoreCase))
            {
                // Prevent navigation to the restricted page.
                output.Attributes.RemoveAll("href");
                output.Attributes.SetAttribute("role", "button");
            }
        }
    }
}

