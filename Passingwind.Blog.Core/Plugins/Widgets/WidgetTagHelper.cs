using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	[HtmlTargetElement("widget")]
	public class WidgetTagHelper : TagHelper
	{
		private readonly IWidgetsManager _widgetsManager;
		private readonly IWidgetViewService _widgetViewService;

		[HtmlAttributeName("position")]
		public string Position { get; set; }

		public WidgetTagHelper(IWidgetsManager widgetsManager, IWidgetViewService widgetViewService)
		{
			_widgetsManager = widgetsManager;
			_widgetViewService = widgetViewService;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.SuppressOutput();

			var widgets = await _widgetsManager.GetWidgetsAsync(Position);

			foreach (var item in widgets)
			{
				var content = await _widgetsManager.GetViewContentAsync(item);
				output.Content.AppendHtml(content);
			}
		}
	}
}
