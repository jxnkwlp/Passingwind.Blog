using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Passingwind.Blog.Plugins
{
	public class PluginLoader : IPluginLoader
	{
		private readonly IHostEnvironment _hostEnvironment;
		private readonly string _pluginRootPath;
		private readonly ILogger<PluginLoader> _logger;

		public PluginLoader(IHostEnvironment hostEnvironment, ILogger<PluginLoader> logger)
		{
			_hostEnvironment = hostEnvironment;
			_logger = logger;

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
				//var mainAssembly = dlls.Select(t => GetAssembly(t.FullName))
				//						.ToArray()
				//						.Where(t => t != null && t.ExportedTypes.Any(c => typeof(IPlugin).IsAssignableFrom(c)))
				//						.Distinct()
				//						.FirstOrDefault();

				var package = GeneratePackage(directoryInfo);

				if (package != null)
					result.Add(package);

			}

			return result;
		}

		private PluginPackage GeneratePackage(DirectoryInfo directoryInfo)
		{
			var dlls = FindDlls(directoryInfo);

			if (dlls == null || dlls.Length == 0)
				return null;

			PluginAssemblyLoadContext loadContext = new PluginAssemblyLoadContext(directoryInfo.FullName, directoryInfo.Name);

			Assembly assembly = null;
			Type pluginType = null;

			foreach (var dllFile in dlls)
			{
				//if (_sharedDlls.Contains(dllFile.Name, StringComparer.InvariantCultureIgnoreCase))
				//	continue;

				// var rrr = new AssemblyDependencyResolver(dllFile.FullName);

				var loadAssembly = LoadAssembly(loadContext, dllFile.FullName);

				if (loadAssembly == null)
					continue;

				var findPluginType = loadAssembly.ExportedTypes.FirstOrDefault(c => typeof(IPlugin).IsAssignableFrom(c) && c.IsClass && !c.IsAbstract && !c.IsInterface);

				if (findPluginType != null)
				{
					pluginType = findPluginType;
					assembly = loadAssembly;
				}
			}

			if (assembly == null || pluginType == null)
				return null;

			return new PluginPackage()
			{
				PluginType = pluginType,
				Assembly = assembly,
				AssemblyName = assembly.ManifestModule.Name.Substring(0, assembly.ManifestModule.Name.LastIndexOf('.')),

				ContentPath = directoryInfo.FullName,
				//RelativePath = "~/" + directoryInfo.FullName.Substring(_hostEnvironment.ContentRootPath.Length + 1).Replace(@"\", "/"),
				// RelativePath = directoryInfo.FullName.Substring(_hostEnvironment.ContentRootPath.Length)
			};
		}

		private FileInfo[] FindDlls(DirectoryInfo directoryInfo)
		{
			var dlls = directoryInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

#if DEBUG
			if (dlls == null || dlls.Count() == 0)
			{
				if (directoryInfo.GetDirectories().Any(t => t.Name == "bin"))
				{
					dlls = directoryInfo.GetDirectories().FirstOrDefault(t => t.Name == "bin").GetFiles("*.dll", SearchOption.AllDirectories);
				}
				else
				{
				}
			}
#endif
			return dlls;
		}

		private Assembly LoadAssembly(PluginAssemblyLoadContext loadContext, string path)
		{
			var assemblyName = AssemblyName.GetAssemblyName(path);

			var defaultAssembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(t => t.FullName == assemblyName.FullName);

			if (defaultAssembly != null)
				return defaultAssembly;

			try
			{
				// TODO  chang to PluginAssemblyLoadContext.LoadFromAssemblyPath(path)
				// return loadContext.LoadFromAssemblyPath(path);

				return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Load plugins assembly file '{0}' faild.", path);
				return null;
			}
		}

		public void Dispose()
		{
		}
	}
}
