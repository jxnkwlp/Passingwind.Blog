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
		public WidgetPackageFinder(ILogger logger, WidgetOptions options)
		{
			Logger = logger;
			Options = options;
		}

		public ILogger Logger { get; }
		public WidgetOptions Options { get; }


		public IEnumerable<WidgetDescriptor> Load(string rootDirectory)
		{
			Logger.LogInformation($"Widgets root path: {rootDirectory} ");

			if (!Directory.Exists(rootDirectory))
				return Enumerable.Empty<WidgetDescriptor>();

			var result = new List<WidgetDescriptor>();

			var loader = new WidgetAssemblyLoader(Logger);

			var searchResult = loader.Search(rootDirectory, Options.ShardTypes ?? new Type[0]);

			foreach (var item in searchResult)
			{
				Logger.LogDebug("Load in directory: {0}", item.Key);

				var directoryInfo = new DirectoryInfo(item.Key);

				try
				{
					var description = LoadDescriptionFromJsonFile(directoryInfo);

					if (description == null)
						continue;

					Logger.LogDebug("Loaded description json file. Widget Id : '{0}' ", description.Id);

					var widgetDescriptor = CreateWidgetDescriptor(description, item.Value);
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

		private WidgetDescriptor CreateWidgetDescriptor(WidgetConfigDescritpion descritpion, IEnumerable<Assembly> assemblies)
		{
			var descriptior = new WidgetDescriptor()
			{
				Author = descritpion.Author,
				Description = descritpion.Description,
				Id = descritpion.Id,
				Name = descritpion.Name,
				Version = descritpion.Version,
			};


			foreach (var assembly in assemblies)
			{
				Logger.LogDebug("Load assembly '{0}' ", assembly.FullName);

				var mainType = assembly.ExportedTypes.FirstOrDefault(t => !t.IsAbstract && t.BaseType != null && typeof(IWidget).IsAssignableFrom(t));

				if (mainType == null)
				{
					continue;
				}

				descriptior.Instance = (IWidget)Activator.CreateInstance(mainType);
				descriptior.Assembly = assembly;

				Logger.LogDebug("Find widget assembly '{0}' ", assembly.FullName);

				descriptior.ComponentType = assembly.ExportedTypes.FirstOrDefault(t => !t.IsAbstract && t.BaseType != null && typeof(WidgetComponent).IsAssignableFrom(t)); ;
			}

			if (descriptior.Assembly == null)
				return null;

			return descriptior;



			//WidgetAssemblyLoadContext loadContext = new WidgetAssemblyLoadContext(directory.FullName);

			//foreach (var item in directory.GetFiles("*.dll"))
			//{
			//	//var assembly = loadContext.LoadFromAssemblyPath(item.FullName);
			//	var assembly = LoadAssembly(item.FullName);

			//	if (assembly == null)
			//		continue;

			//	Logger.LogDebug("Load assembly file '{0}' ", item.FullName);

			//	var mainType = assembly.ExportedTypes.FirstOrDefault(t => !t.IsAbstract && t.BaseType != null && (t.BaseType.Name == "WidgetBase" || t.BaseType.Name == "IWidget" || typeof(IWidget).IsAssignableFrom(t)));

			//	if (mainType == null)
			//	{
			//		continue;
			//	}

			//	descriptior.Instance = (IWidget)Activator.CreateInstance(mainType);
			//	descriptior.Assembly = assembly;

			//	Logger.LogDebug("Find widget assembly '{0}' ", assembly.FullName);

			//	var viewComponentType = assembly.ExportedTypes.FirstOrDefault(t => !t.IsAbstract && t.BaseType != null && t.BaseType.Name == "WidgetComponent");

			//	descriptior.ComponentType = viewComponentType;
			//}

			//if (descriptior.Assembly == null)
			//	return null;

			//return descriptior;
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
