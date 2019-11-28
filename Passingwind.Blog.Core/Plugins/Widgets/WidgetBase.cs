using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public abstract class WidgetBase : PluginBase, IWidget
	{
		private readonly IWidgetViewService _widgetViewService;

		public WidgetBase(IWidgetViewService widgetViewService)
		{
			_widgetViewService = widgetViewService;
		}

		public string ViewPath => "Views/Default.cshtml";

		protected virtual Task<object> GetViewDataAsync()
		{
			return null;
		}

		public async virtual Task<string> GetViewContentAsync()
		{
			var data = await GetViewDataAsync();
			return _widgetViewService.RenderView(Description, ViewPath);
		}
	}
}
