using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Themes
{
	public class ThemeService : IThemeService
	{
		private const string _rootName = "Themes";
		private const string _configJson = "theme.json";
		private const string _themeNameCacheKey = "theme.default";

		private readonly ILogger<ThemeService> _logger;
		private readonly IHostEnvironment _hostEnvironment;
		private readonly IMemoryCache _memoryCache;
		private readonly IThemeContainer _themeContainer;

		public ThemeService(IHostEnvironment hostEnvironment, ILogger<ThemeService> logger, IMemoryCache memoryCache, IThemeContainer themeContainer)
		{
			_hostEnvironment = hostEnvironment;
			_memoryCache = memoryCache;
			_logger = logger;
			_themeContainer = themeContainer;
		}

		public Task<IEnumerable<ThemeDescriptor>> GetThemeDescriptorsAsync()
		{
			var list = _themeContainer.Themes;
			return Task.FromResult<IEnumerable<ThemeDescriptor>>(list.ToArray());
		}

		public async Task SetDefaultThemeAsync(string name)
		{
			string rootPath = Path.Combine(_hostEnvironment.ContentRootPath, _rootName);

			var configJsonFile = Path.Combine(rootPath, _configJson);

			try
			{
				var themeConfig = new ThemeConfig()
				{
					Name = name.Trim(),
				};

				var jsonString = JsonSerializer.Serialize(themeConfig);

				await File.WriteAllTextAsync(configJsonFile, jsonString);

				_memoryCache.Set(_themeNameCacheKey, name);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Save theme config failed.");
			}
		}

		public Task<string> GetDefaultThemeAsync()
		{
			string rootPath = Path.Combine(_hostEnvironment.ContentRootPath, _rootName);

			var configJsonFile = Path.Combine(rootPath, _configJson);

			return _memoryCache.GetOrCreateAsync(_themeNameCacheKey, (_) =>
			{
				try
				{
					if (!File.Exists(configJsonFile))
						return Task.FromResult<string>(null);

					var jsonString = File.ReadAllText(configJsonFile);

					var config = JsonSerializer.Deserialize<ThemeConfig>(jsonString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

					_memoryCache.Set(_themeNameCacheKey, config?.Name);

					return Task.FromResult(config?.Name);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Load theme config failed.");
				}

				return Task.FromResult<string>("Default");
			});
		}

		public class ThemeConfig
		{
			public string Name { get; set; }
		}
	}
}
