using Passingwind.Blog.Json;
using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.LinkList.Models;
using Passingwind.Blog.Widgets;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.LinkList
{
	public class LinkListWidgetView : WidgetComponent
	{
		private readonly IWidgetDynamicContentService _widgetContentService;
		private readonly IJsonSerializer _jsonSerializer;

		public LinkListWidgetView(IWidgetDynamicContentService widgetSettingsService, IJsonSerializer jsonSerializer)
		{
			_widgetContentService = widgetSettingsService;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			var widgetId = WidgetViewContext.ConfigurationInfo.Id;

			var list = await _widgetContentService.GetListAsync<LinkModel>(widgetId, string.Empty);

			var model = new WidgetViewModel()
			{
				Title = WidgetViewContext.ConfigurationInfo.Title,
				List = list,
			};

			return View(model);
		}
	}
}
