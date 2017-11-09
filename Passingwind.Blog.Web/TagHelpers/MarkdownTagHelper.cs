using CommonMark;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.TagHelpers
{
    [HtmlTargetElement("markdown", TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement(Attributes = "asp-markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-content")]
        public ModelExpression Content { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output.TagName == "markdown")
            {
                output.TagName = null;
            }

            var content = await GetContent(output);
            var markdown = content;
            var html = MarkdownHelper.CoventToHtml(markdown);
            output.Content.SetHtmlContent(html.Value);

        }

        private async Task<string> GetContent(TagHelperOutput output)
        {
            if (Content == null)
                return (await output.GetChildContentAsync()).GetContent();

            return Content.Model?.ToString();
        }
    }
}
