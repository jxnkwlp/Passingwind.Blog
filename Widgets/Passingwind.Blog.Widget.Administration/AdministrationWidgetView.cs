using Passingwind.Blog.Widget.Administration.Models;
using Passingwind.Blog.Widgets;

namespace Passingwind.Blog.Widget.Administration
{
	public class AdministrationWidgetView : WidgetComponent
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
