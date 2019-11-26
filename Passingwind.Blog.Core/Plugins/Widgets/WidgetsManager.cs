using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetsManager : IWidgetsManager
	{
		const string fileName = "widgets.json";

		private readonly IHostEnvironment _hostEnvironment;
		private readonly string _widgetFile;
		private readonly Dictionary<string, string[]> _widgets = new Dictionary<string, string[]>();
		private readonly IWidgetViewService _widgetViewService;
		private readonly IPluginManager _pluginManager;

		public WidgetsManager(IHostEnvironment hostEnvironment, IWidgetViewService widgetViewService, IPluginManager pluginManager)
		{
			_hostEnvironment = hostEnvironment;
			_widgetViewService = widgetViewService;

			_widgetFile = Path.Combine(hostEnvironment.ContentRootPath, "App_Data", fileName);

			ParseConfig();
			_pluginManager = pluginManager;
		}

		protected void ParseConfig()
		{
			if (!File.Exists(_widgetFile))
				return;

			try
			{
				string content = File.ReadAllText(_widgetFile);

				var result = JsonSerializer.Deserialize<Dictionary<string, string[]>>(content);

				foreach (var item in result)
				{
					_widgets[item.Key] = item.Value;
				}
			}
			catch (Exception)
			{
			}
		}

		public async Task<string> GetViewContentAsync(string name)
		{
			var plugin = _pluginManager.GetPlugin(name);

			if (plugin == null)
				return null;

			if (plugin is IWidget widget)
			{
				return await widget.GetViewContentAsync();
			}

			return null;
		}

		public Task<IEnumerable<string>> GetWidgetsAsync(string position)
		{
			if (_widgets.ContainsKey(position))
				return Task.FromResult<IEnumerable<string>>(_widgets[position]);

			return Task.FromResult<IEnumerable<string>>(null);
		}
	}
}
