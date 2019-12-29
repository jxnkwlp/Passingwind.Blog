using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	[HtmlTargetElement("widget-zone")]
	public class WidgetZoneTagHelper : TagHelper
	{
		private readonly IWidgetConfigService _widgetConfigService;
		private readonly IWidgetViewService _widgetViewService;
		private readonly IWidgetsManager _widgetsManager;

		[HtmlAttributeName("position")]
		public string Position { get; set; }

		public WidgetZoneTagHelper(IWidgetConfigService widgetConfigService, IWidgetViewService widgetViewService, IWidgetsManager widgetsManager)
		{
			_widgetConfigService = widgetConfigService;
			_widgetViewService = widgetViewService;
			_widgetsManager = widgetsManager;
		}


		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.SuppressOutput();
#if DEBUG
			output.PreContent.AppendHtml($"<!-- widget position '{Position}' -->");
#endif

			var widgets = await _widgetConfigService.GetByPositionAsync(Position);

			if (widgets != null)
				foreach (var item in widgets)
				{
#if DEBUG
					output.Content.AppendHtml(System.Environment.NewLine + $"<!-- widget '{item.Name}' -->");
#endif
					var descriptor = await _widgetsManager.GetWidgetDescriptorAsync(item.Name, item.Id);

					if (descriptor != null)
					{ 
						var content = await _widgetViewService.GetViewContentAsync(descriptor);

						if (!string.IsNullOrWhiteSpace(content))
							output.Content.AppendHtml(content);
					}
				}
		}
	}
}
