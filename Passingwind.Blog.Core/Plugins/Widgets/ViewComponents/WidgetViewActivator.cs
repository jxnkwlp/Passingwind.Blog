using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public class WidgetViewActivator : IWidgetViewActivator
	{
		private readonly ILogger<WidgetViewActivator> _logger;
		private readonly IServiceProvider _serviceProvider;

		public WidgetViewActivator(IServiceProvider serviceProvider, ILogger<WidgetViewActivator> logger)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
		}

		public object Create(WidgetDescriptor widgetDescriptor)
		{
			var widgetViewType = widgetDescriptor.Assembly.ExportedTypes.FirstOrDefault(t => typeof(WidgetView).GetTypeInfo().IsAssignableFrom(t));

			if (widgetViewType != null)
			{
				var t = _serviceProvider.CreateScope().ServiceProvider.GetService(widgetViewType);

				var widgetViewInstance = _serviceProvider.CreateScope().ServiceProvider.GetService(widgetViewType) as WidgetView;

				return widgetViewInstance;
			}

			return null;
		}

	}
}
