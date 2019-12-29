using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public interface IWidgetViewResult
	{
		string ViewPath { get; }

		ViewDataDictionary ViewData { get; }
	}
}
