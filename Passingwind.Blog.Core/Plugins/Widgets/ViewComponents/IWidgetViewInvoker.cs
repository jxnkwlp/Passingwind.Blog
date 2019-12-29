using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public interface IWidgetViewInvoker
	{
		Task<IWidgetViewResult> InvokeAsync(WidgetViewContext context);
	}
}