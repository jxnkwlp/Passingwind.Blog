using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Passingwind.Blog.Widgets
{
	internal class WidgetsManager : IWidgetsManager
	{
		private readonly IWidgetFactory _widgetApplication;
		private readonly WidgetOptions _options;

		public WidgetsManager(IWidgetFactory widgetApplication, IOptions<WidgetOptions> options)
		{
			_widgetApplication = widgetApplication;
			_options = options.Value;
		}

		public WidgetsManager(IWidgetFactory widgetApplication, WidgetOptions options)
		{
			_widgetApplication = widgetApplication;
			_options = options;
		}

		public void Dispose()
		{
		}

		public void Register(IServiceCollection services)
		{
			using (var scope = services.BuildServiceProvider().CreateScope())
			{
				var manager = scope.ServiceProvider.GetRequiredService<IApplicationWidgetPartManager>();
				var widgets = scope.ServiceProvider.GetRequiredService<IWidgetContainer>().Widgets;

				// TODO check widget is installed. 
				AddToApplicationPart(manager, widgets);
			}
		}

		public void Initialize(IApplicationBuilder app)
		{
			var manager = app.ApplicationServices.GetRequiredService<IApplicationWidgetPartManager>();
			var widgets = app.ApplicationServices.GetRequiredService<IWidgetContainer>().Widgets;
			var hostEnvironment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
			var logger = app.ApplicationServices.GetRequiredService<ILogger<WidgetsManager>>();


			var services = _widgetApplication.Services;

			// TODO check widget is installed. 
			AddToApplicationPart(manager, widgets);


			foreach (var item in widgets)
			{
				if (item.Instance != null)
				{
					item.Instance.Configure(new WidgetConfigureContext(app.ApplicationServices));
				}

				var wwwrootFolder = Path.Combine(hostEnvironment.ContentRootPath, _options.Directory, item.Id, "wwwroot");
				if (Directory.Exists(wwwrootFolder))
				{
					logger.LogInformation("Use '{0}' as static files folder.", wwwrootFolder);

					app.UseStaticFiles(new StaticFileOptions()
					{
						RequestPath = new Microsoft.AspNetCore.Http.PathString($"/{item.Id.ToLower()}"),
						FileProvider = new PhysicalFileProvider(wwwrootFolder), 
					});
				}
			}
		}

		private void AddToApplicationPart(IApplicationWidgetPartManager manager, IEnumerable<WidgetDescriptor> widgets)
		{
			foreach (var item in widgets)
			{
				manager.Add(item.Assembly);
			}
		}

	}
}
