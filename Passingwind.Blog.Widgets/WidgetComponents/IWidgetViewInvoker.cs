using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public interface IWidgetViewInvoker
	{
		Task InvokeAsync(WidgetViewContext context);
	}
}
