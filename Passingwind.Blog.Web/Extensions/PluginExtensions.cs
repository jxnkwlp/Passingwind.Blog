using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;

namespace Passingwind.Blog.Web
{
	public static class PluginExtensions
	{
		public static IMvcBuilder AddPlugins(this IMvcBuilder mvcBuilder)
		{
			ConfigServices(mvcBuilder.Services);

			var services = mvcBuilder.Services.BuildServiceProvider();

			var manager = services.GetRequiredService<IPluginManager>();

			manager.LoadPlugins(mvcBuilder.PartManager);

			return mvcBuilder;
		}

		private static void ConfigServices(IServiceCollection services)
		{
			services.AddSingleton<IPluginLoader, PluginLoader>();
			services.AddSingleton<IPluginManager, PluginManager>();

			services.AddSingleton<IWidgetsManager, WidgetsManager>();
			services.AddTransient<IWidgetViewService, WidgetViewService>();
		}
	}
}
