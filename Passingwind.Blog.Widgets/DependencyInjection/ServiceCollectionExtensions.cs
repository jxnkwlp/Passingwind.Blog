using Passingwind.Blog.Widgets;
using System;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
	public static class ServiceCollectionExtensions
	{ 
		public static IServiceCollection AddWidgets(this IServiceCollection services, Action<WidgetOptions> action = null)
		{
			var options = new WidgetOptions();
			action?.Invoke(options);

			new WidgetFactory(services, options);

			//var mvcBuilder = services.AddControllersWithViews();
			//services.AddSingleton<ITypeActivatorCache, TypeActivatorCache>();
			//services.AddSingleton<IApplicationWidgetPartManager, ApplicationWidgetPartManager>();


			//services.AddSingleton<WidgetPackageLoader>();
			//services.AddScoped<IWidgetViewInvoker, WidgetViewInvoker>();
			//services.AddScoped<IWidgetFactory, WidgetFactory>();
			//services.AddScoped<IWidgetComponentActivator, WidgetComponentActivator>();


			//using (var scope = services.BuildServiceProvider().CreateScope())
			//{
			//	var loader = scope.ServiceProvider.GetRequiredService<WidgetPackageLoader>();
			//	var manager = scope.ServiceProvider.GetRequiredService<IApplicationWidgetPartManager>();
			//	var configurationService = scope.ServiceProvider.GetRequiredService<IWidgetConfigurationService>();

			//	var widgets = loader.Load();

			//	RegisterWidgets(services, manager, configurationService, widgets);
			//}

			return services;
		}

		//private static void RegisterWidgets(IServiceCollection services, IApplicationWidgetPartManager manager, IWidgetConfigurationService configurationService, IEnumerable<WidgetDescriptor> widgets)
		//{
		//	foreach (var item in widgets)
		//	{
		//		services.AddScoped(item.ComponentType);
		//		services.AddScoped(item.Type);
		//		services.AddScoped(typeof(IWidget), item.Type);
		//	}

		//	foreach (var item in widgets)
		//	{
		//		manager.Add(item.Assembly);
		//	}
		//}

		//private static void AddWidgetToAppcationPart(ApplicationPartManager manager, IEnumerable<WidgetDescriptor> widgets)
		//{
		//	foreach (var item in widgets)
		//	{
		//		var assembly = item.Assembly;

		//		AddToAppcationPart(manager, assembly);
		//	}
		//}

		//private static void AddToAppcationPart(ApplicationPartManager manager, Assembly assembly)
		//{
		//	var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);

		//	var applicationParts = partFactory.GetApplicationParts(assembly);

		//	foreach (var part in applicationParts)
		//	{
		//		if (part is AssemblyPart assemblyPart)
		//		{
		//			manager.ApplicationParts.Add(new WidgetAssemblyPart(assembly));
		//		}
		//		else if (part is CompiledRazorAssemblyPart)
		//		{
		//			manager.ApplicationParts.Add(new WidgetRazorAssemblyPart(new CompiledRazorAssemblyPart(assembly)));
		//		}
		//		else
		//		{
		//			manager.ApplicationParts.Add(part);
		//		}
		//	}

		//	// load related assemblies
		//	var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, true);

		//	foreach (var item in relatedAssemblies)
		//	{
		//		AddToAppcationPart(manager, item);
		//	}
		//}
	}
}
