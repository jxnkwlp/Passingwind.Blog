using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Passingwind.Blog.Widgets
{
	public class WidgetPackageFinder
	{
		public WidgetPackageFinder(IHostEnvironment hostEnvironment, ILogger logger)
		{
			HostEnvironment = hostEnvironment;
			Logger = logger;
		}

		public IHostEnvironment HostEnvironment { get; }
		public ILogger Logger { get; }

		public IEnumerable<WidgetDescriptor> Find(string directory)
		{
			string rootDirectory = Path.Combine(HostEnvironment.ContentRootPath, directory);

			Logger.LogInformation($"Widgets root path: {rootDirectory} ");

			if (!Directory.Exists(rootDirectory))
				return Enumerable.Empty<WidgetDescriptor>();

			var pluginDirectoryInfo = new DirectoryInfo(rootDirectory);

			var result = new List<WidgetDescriptor>();

			foreach (var directoryInfo in pluginDirectoryInfo.GetDirectories())
			{
				Logger.LogDebug("Search in directory: {0}", directoryInfo.FullName);

				try
				{
					var description = LoadDescriptionFromJsonFile(directoryInfo);

					if (description == null)
						continue;

					Logger.LogDebug("Loaded description json file. Widget Id : '{0}' ", description.Id);

					var widgetDescriptor = CreateWidgetDescriptor(description, directoryInfo);
					if (widgetDescriptor != null)
						result.Add(widgetDescriptor);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, $"Try load widget failed in directory '{directoryInfo.FullName}' ");
				}
			}

			return result;
		}

		private WidgetDescriptor CreateWidgetDescriptor(WidgetConfigDescritpion descritpion, DirectoryInfo directory)
		{
			var descriptior = new WidgetDescriptor()
			{
				Author = descritpion.Author,
				Description = descritpion.Description,
				Id = descritpion.Id,
				Name = descritpion.Name,
				Version = descritpion.Version,
			};

			PluginAssemblyLoadContext loadContext = new PluginAssemblyLoadContext(directory.FullName);

			foreach (var item in directory.GetFiles("*.dll"))
			{
				//var assembly = loadContext.LoadFromAssemblyPath(item.FullName);
				var assembly = LoadAssembly(item.FullName);

				if (assembly == null)
					continue;

				Logger.LogDebug("Load assembly file '{0}' ", item.FullName);

				var mainType = assembly.ExportedTypes.FirstOrDefault(t => !t.IsAbstract && t.BaseType != null && (t.BaseType.Name == "WidgetBase" || t.BaseType.Name == "IWidget" || typeof(IWidget).IsAssignableFrom(t)));

				if (mainType == null)
				{
					continue;
				}

				descriptior.Instance = (IWidget)Activator.CreateInstance(mainType);
				descriptior.Assembly = assembly;

				Logger.LogDebug("Find widget assembly '{0}' ", assembly.FullName);

				var viewComponentType = assembly.ExportedTypes.FirstOrDefault(t => !t.IsAbstract && t.BaseType != null && t.BaseType.Name == "WidgetComponent");

				descriptior.ComponentType = viewComponentType;
			}

			if (descriptior.Assembly == null)
				return null;

			return descriptior;
		}

		private static Assembly LoadAssembly(string path)
		{
			var assemblyName = AssemblyName.GetAssemblyName(path);

			var defaultAssembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(t => t.FullName == assemblyName.FullName);

			if (defaultAssembly != null)
				return defaultAssembly;

			try
			{
				// TODO  chang to PluginAssemblyLoadContext.LoadFromAssemblyPath(path)
				// return loadContext.LoadFromAssemblyPath(path);

				if (path.EndsWith(".Views.dll"))
				{
					var mainDllPath = path.Replace(".Views.dll", ".dll");

					LoadAssembly(mainDllPath);
				}

				return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static WidgetConfigDescritpion LoadDescriptionFromJsonFile(DirectoryInfo directory)
		{
			var file = Path.Combine(directory.FullName, "plugin.json");
			if (!File.Exists(file))
				return null;

			try
			{
				var content = File.ReadAllText(file);

				return JsonSerializer.Deserialize<WidgetConfigDescritpion>(content, JsonHelper.Options);
			}
			catch (Exception)
			{
			}

			return null;
		}


		public class WidgetConfigDescritpion
		{
			public string Id { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string Author { get; set; }
			public string Version { get; set; }
		}
	}
}
