using Microsoft.Extensions.Hosting;
using Passingwind.Blog.Json;
using Passingwind.Blog.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Services
{
	public class WidgetManager : IWidgetManager
	{
		private const string _configFileName = "widgets.json";

		private readonly string _configFile = null;
		private Dictionary<string, IList<WidgetPositionConfigModel>> _widgetConfigurationData = new Dictionary<string, IList<WidgetPositionConfigModel>>();

		private readonly IHostEnvironment _hostEnvironment;
		private readonly IJsonSerializer _jsonSerializer;
		private readonly IServiceProvider _serviceProvider;
		private readonly IWidgetContainer _widgetContainer;

		private readonly FileSystemWatcher _fileSystemWatcher;

		public WidgetManager(IHostEnvironment hostEnvironment, IJsonSerializer jsonSerializer, IServiceProvider serviceProvider, IWidgetContainer widgetContainer)
		{
			_hostEnvironment = hostEnvironment;
			_jsonSerializer = jsonSerializer;
			_serviceProvider = serviceProvider;

			_configFile = Path.Combine(hostEnvironment.ContentRootPath, "App_Data", _configFileName);

			LoadFromConfigFile();

			_fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(_configFile))
			{
				EnableRaisingEvents = true
			};
			_fileSystemWatcher.Changed += FileSystemWatcher_Changed;
			_widgetContainer = widgetContainer;
		}

		private void LoadFromConfigFile()
		{
			if (!File.Exists(_configFile))
				return;

			try
			{
				var jsonContent = File.ReadAllText(_configFile);
				_widgetConfigurationData = _jsonSerializer.Deserialize<Dictionary<string, IList<WidgetPositionConfigModel>>>(jsonContent);
			}
			catch (Exception)
			{
			}
		}

		private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (e.FullPath != _configFile)
				return;

			LoadFromConfigFile();
		}

		private async Task SaveConfigFileAsync()
		{
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(_configFile)))
					Directory.CreateDirectory(Path.GetDirectoryName(_configFile));

				var json = _jsonSerializer.Serialize(_widgetConfigurationData);

				await File.WriteAllTextAsync(_configFile, json);

				Thread.Sleep(10);
			}
			catch (Exception)
			{
			}
		}

		public Task<IEnumerable<WidgetConfigurationModel>> GetWidgetsAsync()
		{
			var result = _widgetConfigurationData.SelectMany(t => t.Value).Select(t => new WidgetConfigurationModel()
			{
				Id = t.Id,
				Title = t.Name,
				WidgetId = t.WidgetId,
				WidgetName = t.Name,
				Zone = null,
			});

			return Task.FromResult(result);
		}

		public Task<IEnumerable<WidgetConfigurationModel>> GetWidgetsByZoneAsync(string name)
		{
			if (_widgetConfigurationData.TryGetValue(name, out var widgets))
			{
				var result = widgets.Select(t => new WidgetConfigurationModel()
				{
					Id = t.Id,
					Title = t.Name,
					WidgetId = t.WidgetId,
					WidgetName = t.Name,
					Zone = null,
				});

				return Task.FromResult(result);
			}

			return Task.FromResult(Enumerable.Empty<WidgetConfigurationModel>());
		}

		public async Task AddToZoneAsync(string zone, WidgetPositionConfigModel config)
		{
			config.Id = config.Id == Guid.Empty ? Guid.NewGuid() : config.Id;

			if (!_widgetConfigurationData.ContainsKey(zone))
				_widgetConfigurationData[zone] = new List<WidgetPositionConfigModel>();

			_widgetConfigurationData[zone].Add(config);

			await SaveConfigFileAsync();
		}

		public async Task RemoveFromZoneAsync(string zone, WidgetPositionConfigModel config)
		{
			var widgetZones = _widgetConfigurationData;

			if (widgetZones.ContainsKey(zone) && widgetZones[zone].Any(t => t.Id == config.Id))
			{
				var find = widgetZones[zone].First(t => t.Id == config.Id);

				widgetZones[zone].Remove(find);
			}

			await SaveConfigFileAsync();
		}

		public async Task RemoveAsync(string widgetId)
		{
			var widgetZones = _widgetConfigurationData;

			foreach (var item in widgetZones)
			{
				if (widgetZones[item.Key].Any(t => t.WidgetId == widgetId))
				{
					var find = widgetZones[item.Key].First(t => t.WidgetId == widgetId);

					widgetZones[item.Key].Remove(find);
				}
			}

			await SaveConfigFileAsync();
		}

		public Task<IEnumerable<string>> GetInstalledAsync()
		{
			return Task.FromResult(_widgetContainer.Widgets.Select(t => t.Name));
		}

		public Task<Dictionary<string, IEnumerable<WidgetPositionConfigModel>>> GetZoneConfigListAsync()
		{
			return Task.FromResult(_widgetConfigurationData.ToDictionary(t => t.Key, t => t.Value.AsEnumerable()));
		}

		public Task InstallAsync(string id)
		{
			// TODO 
			throw new NotImplementedException();
		}

		public Task UninstallAsync(string id)
		{
			// TODO 
			throw new NotImplementedException();
		}

		public async Task ClearFromZoneAsync(string zone)
		{
			if (_widgetConfigurationData.ContainsKey(zone))
			{
				_widgetConfigurationData[zone].Clear();

				await SaveConfigFileAsync();
			}

		}
	}

}
