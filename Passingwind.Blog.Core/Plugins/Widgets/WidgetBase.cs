using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public abstract class WidgetBase : IWidget
	{
		private readonly IWidgetViewService _widgetViewService;

		public WidgetBase(IWidgetViewService widgetViewService)
		{
			_widgetViewService = widgetViewService;
		}

		public string ViewName => "Default";

		protected virtual Task<object> GetViewDataAsync()
		{
			return null;
		}

		public async virtual Task<string> GetViewContentAsync()
		{
			var data = await GetViewDataAsync();
			return _widgetViewService.RenderView(ViewName, data);
		}
	}
}
