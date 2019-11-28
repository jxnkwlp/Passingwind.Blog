using Microsoft.Extensions.Hosting;
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

		public PluginLoader(IHostEnvironment hostEnvironment)
		{
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

			PluginAssemblyLoadContext loadContext = new PluginAssemblyLoadContext();

			Assembly assembly = null;
			Type pluginType = null;

			foreach (var item in dlls)
			{
				//var a2 = typeof(IPlugin).Assembly;

				var assembly2 = LoadAssembly(loadContext, item.FullName);

				//foreach (var item2 in assembly2.ExportedTypes)
				//{
				//	var b1 = typeof(IPlugin).IsAssignableFrom(item2);
				//	var b2 = typeof(IPlugin).IsAssignableFrom(item2.BaseType);
				//}

				pluginType = assembly2.ExportedTypes.FirstOrDefault(c => typeof(IPlugin).IsAssignableFrom(c) && c.IsClass && !c.IsAbstract && !c.IsInterface);

				if (pluginType != null)
				{
					assembly = assembly2;
					break;
				}
			}

			if (assembly == null)
				return null;

			return (new PluginPackage()
			{
				PluginType = pluginType,
				Assembly = assembly,
				ContentPath = directoryInfo.FullName,
				//RelativePath = "~/" + directoryInfo.FullName.Substring(_hostEnvironment.ContentRootPath.Length + 1).Replace(@"\", "/"),
				RelativePath = directoryInfo.FullName.Substring(_hostEnvironment.ContentRootPath.Length)
			});
		}

		private FileInfo[] FindDlls(DirectoryInfo directoryInfo)
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
				}
			}

			return dlls;
		}

		private Assembly LoadAssembly(PluginAssemblyLoadContext loadContext, string path)
		{
			try
			{
				using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					//return loadContext.LoadFromStream(fs);
					return AssemblyLoadContext.Default.LoadFromStream(fs);
				}
				//return Assembly.LoadFile(path);
			}
			catch (Exception)
			{
			}
			return null;
		}

		public void Dispose()
		{
		}
	}
}
