using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;

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
				var hostEnvironment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

				var plugins = manager.GetAllPlugins();

				ConfigPlugins(plugins, mvcBuilder.Services, mvcBuilder.PartManager);
			}

			mvcBuilder.AddRazorRuntimeCompilation();

			// AssemblyLoadContext.Default.Resolving += Default_Resolving;

			return mvcBuilder;
		}

		private static System.Reflection.Assembly Default_Resolving(AssemblyLoadContext loadContext, AssemblyName assemblyName)
		{
			// if (loadContext.Name == "Default") { }

			var contexts = AssemblyLoadContext.All;
			var currentContext = AssemblyLoadContext.CurrentContextualReflectionContext;

			var ass = loadContext.Assemblies.FirstOrDefault(t => t.FullName == assemblyName.FullName);

			return null;
		}

		private static void ConfigPlugins(IEnumerable<PluginDescriptor> plugins, IServiceCollection services, ApplicationPartManager partManager)
		{
			foreach (var item in plugins)
			{
				ConfigPluginsServices(item, services);

				AddPluginToPart(item, partManager);
			}
		}

		private static void ConfigPluginsServices(PluginDescriptor pluginDescriptor, IServiceCollection services)
		{
			// TODO support DI ?
			var pluginInstance = Activator.CreateInstance(pluginDescriptor.PluginType) as IPlugin;
			pluginInstance.PostConfigureServices(services);

			services.AddTransient(pluginDescriptor.PluginType, (s) => pluginInstance);
		}

		private static void AddPluginToPart(PluginPackage pluginPackage, ApplicationPartManager applicationPartManager)
		{
			var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginPackage.Assembly);

			var applicationParts = partFactory.GetApplicationParts(pluginPackage.Assembly);

			foreach (var part in applicationParts)
			{
				applicationPartManager.ApplicationParts.Add(part);
			}

			// var dependencyContext = DependencyContext.Load(pluginPackage.Assembly);

			var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(pluginPackage.Assembly, true);

			foreach (var assembly in relatedAssemblies)
			{
				// CompiledRazorAssemblyApplicationPartFactory.

				partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
				foreach (var part in partFactory.GetApplicationParts(assembly))
				{
					applicationPartManager.ApplicationParts.Add(part);
				}
			}
		}

		private static void ConfigServices(IServiceCollection services)
		{
			services.AddSingleton<IPluginLoader, PluginLoader>();
			services.AddSingleton<IPluginManager, PluginManager>();

			services.AddSingleton<IWidgetConfigService, WidgetConfigService>();
			services.AddSingleton<IWidgetsManager, WidgetsManager>();

			services.AddTransient<IPluginViewRenderService, PluginViewRenderService>();
		}
	}
}
