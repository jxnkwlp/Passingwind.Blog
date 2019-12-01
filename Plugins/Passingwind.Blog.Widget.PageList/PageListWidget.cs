using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Plugins.Widgets;

namespace Passingwind.Blog.Widget.PageList
{
	public class PageListWidget : WidgetBase
	{
		public override void PostConfigureServices(IServiceCollection services)
		{
			services.AddScoped<IWidgetService, WidgetService>();
		}
	}
}
