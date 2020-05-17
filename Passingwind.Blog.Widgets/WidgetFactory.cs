using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Passingwind.Blog.Widgets.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog.Widgets
{
	public class WidgetFactory : IWidgetFactory
	{
		public WidgetFactory(IServiceCollection services, WidgetOptions widgetOptions)
		{
			Services = services;
			WidgetOptions = widgetOptions;
			Widgets = LoadWidgets(services, widgetOptions);

			services.AddSingleton<IWidgetFactory>(this);
			services.AddSingleton<IWidgetContainer>(this);

			services.AddSingleton<ITypeActivatorCache, TypeActivatorCache>();
			services.AddSingleton<IApplicationWidgetPartManager, ApplicationWidgetPartManager>();

			var manager = new WidgetsManager(this, widgetOptions);
			services.AddSingleton<IWidgetsManager>((_) => manager);

			services.AddScoped<IWidgetViewInvoker, WidgetViewInvoker>();
			services.AddScoped<IWidgetComponentFactory, WidgetComponentFactory>();
			services.AddScoped<IWidgetComponentActivator, WidgetComponentActivator>();

			ConfigServices(services);
			RegisterWidgets(services, manager);
		}


		public IServiceCollection Services { get; }

		public IServiceProvider ServiceProvider { get; private set; }

		public IReadOnlyCollection<WidgetDescriptor> Widgets { get; private set; }

		public WidgetOptions WidgetOptions { get; }


		public void Initialize(IApplicationBuilder applicationBuilder)
		{
			ServiceProvider = applicationBuilder.ApplicationServices;
			InitialWidgets(applicationBuilder);
		}

		private IReadOnlyList<WidgetDescriptor> LoadWidgets(IServiceCollection services, WidgetOptions options)
		{
			var hostEnvironment = services.GetSingletonInstance<IHostEnvironment>();
			return WidgetPackageFinder.Find(hostEnvironment, options.Directory ?? "widgets").ToList();
		}

		private void RegisterWidgets(IServiceCollection services, IWidgetsManager manager)
		{
			manager.Register(services);
		}

		private void ConfigServices(IServiceCollection services)
		{
			foreach (var widget in Widgets)
			{
				var context = new WidgetConfigureServicesContext(services);
				widget.Instance?.ConfigureServices(context);
			}
		}

		private void InitialWidgets(IApplicationBuilder app)
		{
			using (var scope = ServiceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<IWidgetsManager>().Initialize(app);
			}
		}
	}
}
