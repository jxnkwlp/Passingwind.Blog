using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	[HtmlTargetElement("widget")]
	public class WidgetTagHelper : TagHelper
	{
		private readonly IWidgetsManager _widgetsManager;

		[HtmlAttributeName("position")]
		public string Position { get; set; }

		public WidgetTagHelper(IWidgetsManager widgetsManager)
		{
			_widgetsManager = widgetsManager;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.SuppressOutput();
#if DEBUG
			output.PreContent.AppendHtml($"<!-- widget position '{Position}' -->");
#endif

			var widgets = await _widgetsManager.GetWidgetsAsync(Position);

			if (widgets != null)
				foreach (var item in widgets)
				{
#if DEBUG
					output.Content.AppendHtml(System.Environment.NewLine + $"<!-- widget '{item}' -->");
#endif
					var content = await _widgetsManager.GetViewContentAsync(item.Name);

					if (!string.IsNullOrWhiteSpace(content))
						output.Content.AppendHtml(content);
				}
		}
	}
}
