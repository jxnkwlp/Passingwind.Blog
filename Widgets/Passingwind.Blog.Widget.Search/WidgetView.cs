using Passingwind.Blog.Widget.Search.Models;
using Passingwind.Blog.Widgets;

namespace Passingwind.Blog.Widget.Search
{
	public class WidgetView : WidgetComponent
	{
		public IWidgetComponentViewResult Invoke()
		{
			var model = new WidgetViewModel()
			{
				Title = WidgetViewContext.ConfigurationInfo.Title,
			};

			return View(model);
		}
	}
}
