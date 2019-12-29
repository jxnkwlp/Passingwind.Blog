using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetsManager : IWidgetsManager
	{
		private readonly IWidgetConfigService _widgetConfigService;
		private readonly IPluginManager _pluginManager;

		public WidgetsManager(IWidgetConfigService widgetConfigService, IPluginManager pluginManager)
		{
			_widgetConfigService = widgetConfigService;
			_pluginManager = pluginManager;
		}

		public async Task<WidgetDescriptor> GetWidgetDescriptorAsync(string name, Guid id)
		{
			var widgetConfigInfo = await _widgetConfigService.GetConfigInfoAsync(id);

			var descriptor = _pluginManager.GetPluginDescription(name);

			if (descriptor == null)
				return null;

			var widgetDescriptor = new WidgetDescriptor(descriptor)
			{
				Id = id,
			};

			return widgetDescriptor;
		}
	}
}
