using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Passingwind.Blog.Widgets
{
	public static class WidgetPackageFinder
	{
		public static IEnumerable<WidgetDescriptor> Find(IHostEnvironment hostEnvironment, string directory)
		{
			string rootDirectory = Path.Combine(hostEnvironment.ContentRootPath, directory);

			if (!Directory.Exists(rootDirectory))
				return Enumerable.Empty<WidgetDescriptor>();

			var pluginDirectoryInfo = new DirectoryInfo(rootDirectory);

			var result = new List<WidgetDescriptor>();

			foreach (var directoryInfo in pluginDirectoryInfo.GetDirectories())
			{
				var description = LoadDescriptionFromJsonFile(directoryInfo);

				if (description == null)
					continue;

				var widgetDescriptor = CreateWidgetDescriptor(description, directoryInfo);
				if (widgetDescriptor != null)
					result.Add(widgetDescriptor);
			}

			return result;
		}

		private static WidgetDescriptor CreateWidgetDescriptor(WidgetConfigDescritpion descritpion, DirectoryInfo directory)
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

				var mainType = assembly.ExportedTypes.FirstOrDefault(t => !t.IsAbstract && t.BaseType != null && (t.BaseType.Name == "WidgetBase" || t.BaseType.Name == "IWidget" || typeof(IWidget).IsAssignableFrom(t)));

				if (mainType == null)
				{
					continue;
				}

				descriptior.Instance = (IWidget)Activator.CreateInstance(mainType);
				descriptior.Assembly = assembly;

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
