using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Passingwind.Blog.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement("markdown", TagStructure = TagStructure.NormalOrSelfClosing)]
	[HtmlTargetElement(Attributes = "asp-markdown")]
	public class MarkdownTagHelper : TagHelper
	{
		[HtmlAttributeName("asp-content")]
		public ModelExpression Content { get; set; }

		[HtmlAttributeName("asp-markdown")]
		public bool Enabled { get; set; } = true;

		private readonly IMarkdownService _markdownService;

		public MarkdownTagHelper(IMarkdownService markdownService)
		{
			_markdownService = markdownService;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			if (output.TagName == "markdown")
			{
				output.TagName = "div";
			}

			var content = await GetContent(output);

			if (string.IsNullOrWhiteSpace(content))
				return;

			if (Enabled)
			{
				content = HttpUtility.HtmlDecode(content);

				if (content.Contains("<p") || content.Contains("<div") || content.Contains("<h"))
				{
					return;
				}

				content = content.Trim('\r').Trim('\n').Trim();

				var html = _markdownService.ConventToHtml(content);
				output.Content.SetHtmlContent(html);
			}
		}

		private async Task<string> GetContent(TagHelperOutput output)
		{
			if (Content == null)
				return (await output.GetChildContentAsync()).GetContent(HtmlEncoder.Default);

			return Content.Model?.ToString();
		}
	}
}
