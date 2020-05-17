using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement(Attributes = "asp-html")]
	public class RawHtmlTagHelper : TagHelper
	{
		[HtmlAttributeName("asp-html")]
		public bool IsRawHtml { get; set; } = true;

		private readonly IHtmlHelper _htmlHelper;

		public RawHtmlTagHelper(IHtmlHelper htmlHelper)
		{
			_htmlHelper = htmlHelper;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var hasMarkdownAttribute = context.AllAttributes.ContainsName("asp-markdown");

			if (IsRawHtml)
			{
				var content = (await output.GetChildContentAsync()).GetContent(HtmlEncoder.Default);

				if (string.IsNullOrWhiteSpace(content))
					return;

				content = HttpUtility.HtmlDecode(content);

				if (content.Contains("<p") || content.Contains("<div") || content.Contains("<h"))
				{
					var html = _htmlHelper.Raw(content);
					output.Content.SetHtmlContent(html);
					return;
				}
			}
		}
	}
}
