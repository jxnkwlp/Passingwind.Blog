using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Blog.Widgets
{
	public class WidgetConfigureServicesContext
	{
		public IServiceCollection Services { get; }

		public WidgetConfigureServicesContext(IServiceCollection services)
		{
			Services = services;
		}
	}
}
