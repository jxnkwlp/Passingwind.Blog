using Passingwind.Blog.Json;
using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.CustomHtml.Models;
using Passingwind.Blog.Widgets;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.CustomHtml
{
	public class CustomHtmlWidgetView : WidgetComponent
	{
		private readonly IWidgetDynamicContentService _widgetContentService;
		private readonly IJsonSerializer _jsonSerializer;

		public CustomHtmlWidgetView(IWidgetDynamicContentService widgetSettingsService, IJsonSerializer jsonSerializer)
		{
			_widgetContentService = widgetSettingsService;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			var widgetId = WidgetViewContext.ConfigurationInfo.Id;
			var data = await _widgetContentService.GetAsync<CustomHtmlModel>(widgetId, string.Empty);
			 
			var model = new WidgetViewModel()
			{
				Title = WidgetViewContext.ConfigurationInfo.Title,
				Html = data?.Content,
			};

			return View(model);
		}
	}
}
