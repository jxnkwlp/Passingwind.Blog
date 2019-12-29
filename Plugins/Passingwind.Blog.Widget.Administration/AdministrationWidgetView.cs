using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.Administration
{
	public class AdministrationWidgetView : WidgetView
	{
		public override Task<IWidgetViewResult> InvokeAsync()
		{
			return Task.FromResult(View());
		}
	}
}
