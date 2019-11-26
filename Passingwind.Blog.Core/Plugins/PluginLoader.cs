using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Passingwind.Blog.Plugins
{
	public class PluginLoader : IPluginLoader
	{
		private readonly IHostEnvironment _hostEnvironment;
		private readonly string _pluginRootPath;
		private readonly PluginAssemblyLoadContext _pluginAssemblyLoadContext;

		public PluginLoader(IHostEnvironment hostEnvironment)
		{
			_pluginAssemblyLoadContext = new PluginAssemblyLoadContext();

			_hostEnvironment = hostEnvironment;

			_pluginRootPath = Path.Combine(hostEnvironment.ContentRootPath, "plugins");
		}


		public IEnumerable<PluginPackage> Load()
		{
			if (!Directory.Exists(_pluginRootPath))
				return null;

			var pluginDirectoryInfo = new DirectoryInfo(_pluginRootPath);

			var result = new List<PluginPackage>();

			foreach (var directoryInfo in pluginDirectoryInfo.GetDirectories())
			{
				var dlls = directoryInfo.GetFiles(@"*.dll", SearchOption.TopDirectoryOnly);

				if (dlls == null || dlls.Count() == 0)
				{
					if (directoryInfo.GetDirectories().Any(t => t.Name == "bin"))
					{
						dlls = directoryInfo.GetDirectories().FirstOrDefault(t => t.Name == "bin").GetFiles("*.dll", SearchOption.AllDirectories);
					}
					else
					{
						continue;
					}
				}

				//var mainAssembly = dlls.Select(t => GetAssembly(t.FullName))
				//						.ToArray()
				//						.Where(t => t != null && t.ExportedTypes.Any(c => typeof(IPlugin).IsAssignableFrom(c)))
				//						.Distinct()
				//						.FirstOrDefault();

				Assembly assembly = null;
				Type pluginType = null;

				foreach (var item in dlls)
				{
					var assembly2 = GetAssembly(item.FullName);

					pluginType = assembly2.ExportedTypes.FirstOrDefault(c => typeof(IPlugin).IsAssignableFrom(c));

					if (pluginType != null)
					{
						assembly = assembly2;
						break;
					}
				}



				if (assembly == null)
					continue;

				result.Add(new PluginPackage()
				{
					PluginType = pluginType,
					Assembly = assembly,
					ContentPath = directoryInfo.FullName,
				});
			}

			return result;
		}

		private Assembly GetAssembly(string path)
		{
			try
			{
				//return _pluginAssemblyLoadContext.LoadFromAssemblyPath(path);
				return Assembly.LoadFile(path);
			}
			catch (Exception)
			{
			}
			return null;
		}

		public void Dispose()
		{
			_pluginAssemblyLoadContext?.Unload();
		}
	}
}
