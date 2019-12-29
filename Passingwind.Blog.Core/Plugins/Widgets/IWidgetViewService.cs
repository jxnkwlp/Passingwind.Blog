using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidgetViewService
	{
		Task<string> GetViewContentAsync(WidgetDescriptor widgetDescriptor);
	}
}