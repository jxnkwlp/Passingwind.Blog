using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetConfigService : IWidgetConfigService
	{
		const string fileName = "widgets.json";

		private string _widgetFilePath;
		private readonly Dictionary<string, IList<WidgetConfigInfo>> _widgets = new Dictionary<string, IList<WidgetConfigInfo>>();

		private readonly IHostEnvironment _hostEnvironment;
		private readonly ILogger<WidgetConfigService> _logger;

		private static readonly object _loadConfigFileKey = new object();
		private static bool _fileChanged = true;

		public WidgetConfigService(IHostEnvironment hostEnvironment, ILogger<WidgetConfigService> logger)
		{
			_hostEnvironment = hostEnvironment;
			_logger = logger;

			LoadWidgetsConfig();
		}

		protected void LoadWidgetsConfig()
		{
			_widgetFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "App_Data", "Config", fileName);

			if (!Directory.Exists(Path.GetDirectoryName(_widgetFilePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(_widgetFilePath));

			if (!File.Exists(_widgetFilePath))
				return;

			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(_widgetFilePath), fileName);

			fileSystemWatcher.Changed += (sender, e) =>
			{
				if (e.FullPath == _widgetFilePath)
				{
					_logger.LogTrace("The widget config file has changed.");
					_fileChanged = true;
				}
			};

			fileSystemWatcher.EnableRaisingEvents = true;

			LoadWidgetsConfigFromFile();
		}

		protected void LoadWidgetsConfigFromFile()
		{
			if (_fileChanged)
			{
				lock (_loadConfigFileKey)
				{
					if (_fileChanged)
					{
						try
						{
							string content = File.ReadAllText(_widgetFilePath);

							var result = JsonSerializer.Deserialize<Dictionary<string, IList<WidgetConfigInfo>>>(content);

							foreach (var item in result)
							{
								_widgets[item.Key] = item.Value;
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Load widget config file '{0}' faild.", _widgetFilePath);
						}

						_fileChanged = false;
					}
				}
			}
		}

		protected void TryLoadWidgetsConfigFile()
		{
			if (_fileChanged)
			{
				LoadWidgetsConfigFromFile();
			}
		}

		public Task<Dictionary<string, IEnumerable<WidgetConfigInfo>>> GetAllAsync()
		{
			TryLoadWidgetsConfigFile();

			return Task.FromResult(_widgets.ToDictionary(t => t.Key, t => t.Value.AsEnumerable()));
		}

		public Task<IEnumerable<WidgetConfigInfo>> GetByPositionAsync(string position)
		{
			TryLoadWidgetsConfigFile();

			if (_widgets.ContainsKey(position))
			{
				return Task.FromResult(_widgets[position].OrderBy(t => t.Order).AsEnumerable());
			}

			return Task.FromResult(Enumerable.Empty<WidgetConfigInfo>());
		}

		public async Task AddAsync(string position, WidgetConfigInfo widgetConfigInfo)
		{
			if (!_widgets.ContainsKey(position))
			{
				_widgets[position] = new List<WidgetConfigInfo>();
			}

			_widgets[position].Add(widgetConfigInfo);

			await SaveToFileAsync();
		}

		public async Task RemoveAsync(string position, Guid id)
		{
			if (_widgets.ContainsKey(position))
			{
				var find = _widgets[position].FirstOrDefault(t => t.Id == id);
				if (find != null)
				{
					_widgets[position].Remove(find);
					await SaveToFileAsync();
				}
			}
		}

		public async Task ClearAsync(string position)
		{
			if (_widgets.ContainsKey(position))
			{
				_widgets[position].Clear();

				await SaveToFileAsync();
			}
		}


		public async Task UpdateOrderAsync(string position, Guid id, int order)
		{
			if (_widgets.ContainsKey(position))
			{
				var find = _widgets[position].FirstOrDefault(t => t.Id == id);
				if (find != null)
				{
					find.Order = order;

					await SaveToFileAsync();
				}
			}
		}

		private async Task SaveToFileAsync()
		{
			try
			{
				using (var ms = new MemoryStream())
				{
					await JsonSerializer.SerializeAsync(ms, _widgets);
					await File.WriteAllBytesAsync(_widgetFilePath, (ms.ToArray()));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Save widgets config to file '' failed.", _widgetFilePath);
				throw;
			}
		}

		public Task<WidgetConfigInfo> GetConfigInfoAsync(Guid id)
		{
			foreach (var zone in _widgets)
			{
				foreach (var item in _widgets[zone.Key])
				{
					if (item.Id == id)
						return Task.FromResult(item);
				}
			}

			return Task.FromResult(default(WidgetConfigInfo));
		}
	}
}
