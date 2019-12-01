using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Plugins.Widgets;

namespace Passingwind.Blog.Widget.Tags
{
	public class Widget : WidgetBase
	{
		public override void PostConfigureServices(IServiceCollection services)
		{
			services.AddScoped<IWidgetService, WidgetService>();
		}
	}
}
