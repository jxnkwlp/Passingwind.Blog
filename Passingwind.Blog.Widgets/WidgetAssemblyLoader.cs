using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Passingwind.Blog.Widgets
{
	public class WidgetAssemblyLoader
	{
		private readonly ILogger _logger;

		public WidgetAssemblyLoader(ILogger logger)
		{
			_logger = logger;
		}

		public IDictionary<string, IReadOnlyCollection<Assembly>> Search(string root, params Type[] shardTypes)
		{
			_logger.LogDebug($"Start search and load widgets . ");

			var result = new Dictionary<string, IReadOnlyCollection<Assembly>>();

			var pluginRootDirectoryInfo = new DirectoryInfo(root);

			if (!pluginRootDirectoryInfo.Exists)
				return result;

			foreach (var directoryInfo in pluginRootDirectoryInfo.GetDirectories())
			{
				var assemblies = new List<Assembly>();
				var loadContext = new WidgetAssemblyLoadContext(directoryInfo.FullName, directoryInfo.Name);

				var dllFiles = directoryInfo.GetFiles("*.dll");

				foreach (var file in dllFiles)
				{
					var assembly = LoadAssembly(file.FullName, loadContext, shardTypes);
					if (assembly != null)
						assemblies.Add(assembly);
				}

				if (assemblies.Count > 0)
					result[directoryInfo.FullName] = assemblies;
			}

			return result;
		}

		private Assembly LoadAssembly(string file, AssemblyLoadContext loadContext, params Type[] shardTypes)
		{
			if (file.EndsWith(".Views.dll"))
			{
				var path2 = file.Replace(".Views.dll", ".dll");

				var mainDllAssembly = Assembly.LoadFrom(path2);
				if (!loadContext.Assemblies.Any(t => t.FullName == mainDllAssembly.FullName))
				{
					LoadAssembly(path2, loadContext, shardTypes);
				}
			}

			var assemblyName = Assembly.LoadFrom(file);

			// is in shard types assemblies
			foreach (var type in shardTypes)
			{
				if (assemblyName.ExportedTypes.Any(t => t.FullName == type.FullName))
				{
					return null;
				}
			}

			// is in default assembly load context
			Assembly assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(t => t.FullName == assemblyName.FullName);

			if (assembly != null)
				return assembly;

			// end . load to custom context
#if DEBUG
			var fileStream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
			return loadContext.LoadFromStream(fileStream);
#else
			return loadContext.LoadFromAssemblyPath(file);
#endif

		}
	}
}
