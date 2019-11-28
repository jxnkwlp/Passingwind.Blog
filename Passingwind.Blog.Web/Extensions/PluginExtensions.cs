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

			using (var scope = mvcBuilder.Services.BuildServiceProvider().CreateScope())
			{
				var manager = scope.ServiceProvider.GetRequiredService<IPluginManager>();

				manager.RegisterPlugins(mvcBuilder.Services, mvcBuilder.PartManager);
			}

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
