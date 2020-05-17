using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public interface IWidgetComponentViewResult
	{
		Task ExecuteAsync(WidgetViewContext context);
	}
}
