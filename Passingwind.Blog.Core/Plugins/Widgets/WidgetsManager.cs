using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetsManager : IWidgetsManager
	{
		private readonly Dictionary<string, IWidgetService> _widgetServices = new Dictionary<string, IWidgetService>();
		private readonly IWidgetConfigService _widgetConfigService;
		private readonly IPluginManager _pluginManager;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<WidgetsManager> _logger;

		private static readonly object _findWidgetServiceKey = new object();

		public WidgetsManager(IWidgetConfigService widgetConfigService, IPluginManager pluginManager, IServiceProvider serviceProvider, ILogger<WidgetsManager> logger)
		{
			_widgetConfigService = widgetConfigService;
			_pluginManager = pluginManager;
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public async Task<string> GetViewContentAsync(string name)
		{
			var plugin = _pluginManager.GetPlugin(name);

			if (plugin == null)
				return null;

			if (plugin is IWidget widget)
			{
				if (!_widgetServices.ContainsKey(name))
				{
					lock (_findWidgetServiceKey)
					{
						if (!_widgetServices.ContainsKey(name))
						{
							var allWidgetService = _serviceProvider.CreateScope().ServiceProvider.GetServices<IWidgetService>();
							var t = plugin.GetType();
							var widgetServiceFind = allWidgetService.FirstOrDefault(t => t.GetType().Module == (plugin.GetType().Module));

							_widgetServices[name] = widgetServiceFind;
						}
					}
				}

				var widgetService = _widgetServices[name];
				if (widgetService == null)
				{
					_logger.LogTrace("Not found the widgetService in '{0}'", name);
					return null;
				}

				var descriptor = _pluginManager.GetPluginDescription(name);

				return await widgetService.GetViewContentAsync(descriptor);
			}

			return null;
		}

		public async Task<IEnumerable<WidgetConfigInfo>> GetWidgetsAsync(string position)
		{
			var list = await _widgetConfigService.GetByPositionAsync(position);
			if (list != null)
				return list.OrderBy(t => t.Order).ToList();

			return Enumerable.Empty<WidgetConfigInfo>();
		}
	}
}
