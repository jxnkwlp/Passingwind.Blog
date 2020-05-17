//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.Loader;
//using System.Text.Json;

//namespace Passingwind.Blog.Widgets
//{
//	public class WidgetPackageLoader : IWidgetPackageLoader
//	{
//		public IHostEnvironment HostEnvironment => ServiceProvider.GetRequiredService<IHostEnvironment>();
//		public IServiceProvider ServiceProvider { get; set; }

//		public string DirectoryName { get; set; } = "widgets";

//		public IEnumerable<WidgetDescriptor> Load(IServiceCollection services, WidgetOptions widgetOptions)
//		{
//			if (ServiceProvider == null)
//				throw new Exception("The ServiceProvider is null.");

//			string rootDirectory = Path.Combine(HostEnvironment.ContentRootPath, DirectoryName);

//			if (!Directory.Exists(rootDirectory))
//				return Enumerable.Empty<WidgetDescriptor>();

//			var pluginDirectoryInfo = new DirectoryInfo(rootDirectory);

//			var result = new List<WidgetDescriptor>();

//			foreach (var directoryInfo in pluginDirectoryInfo.GetDirectories())
//			{
//				var description = LoadDescriptionFromJsonFile(directoryInfo);

//				if (description == null)
//					continue;

//				var widgetDescriptor = CreateWidgetDescriptor(description, directoryInfo);

//				ConfigServices(services, widgetDescriptor, widgetOptions);

//				result.Add(widgetDescriptor);
//			}

//			return result;
//		}

//		private void ConfigServices(IServiceCollection services, WidgetDescriptor widget, WidgetOptions widgetOptions)
//		{
//			var context = new WidgetConfigureServicesContext(services, widgetOptions.DbContextOptions);
//			widget.Instance?.ConfigureServices(context);
//		}

//		private WidgetDescriptor CreateWidgetDescriptor(WidgetConfigDescritpion descritpion, DirectoryInfo directory)
//		{
//			var descriptior = new WidgetDescriptor()
//			{
//				Author = descritpion.Author,
//				Description = descritpion.Description,
//				Id = descritpion.Id,
//				Name = descritpion.Name,
//				Version = descritpion.Version,
//			};

//			PluginAssemblyLoadContext loadContext = new PluginAssemblyLoadContext(directory.FullName);

//			foreach (var item in directory.GetFiles("*.dll"))
//			{
//				//var assembly = loadContext.LoadFromAssemblyPath(item.FullName);
//				var assembly = LoadAssembly(item.FullName);

//				var viewComponentType = assembly.ExportedTypes.FirstOrDefault(t => t.BaseType != null && t.BaseType.Name == "WidgetComponent");

//				if (viewComponentType != null)
//				{
//					descriptior.Assembly = assembly;
//					descriptior.ComponentType = viewComponentType;

//					var mainType = assembly.ExportedTypes.FirstOrDefault(t => t.BaseType != null && (t.BaseType.Name == "WidgetBase" || t.BaseType.Name == "IWidget" || typeof(IWidget).IsAssignableFrom(t)));
//					if (mainType != null)
//					{
//						descriptior.Instance = (IWidget)Activator.CreateInstance(mainType);
//					}
//				}
//			}

//			return descriptior;
//		}

//		private Assembly LoadAssembly(string path)
//		{
//			var assemblyName = AssemblyName.GetAssemblyName(path);

//			var defaultAssembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(t => t.FullName == assemblyName.FullName);

//			if (defaultAssembly != null)
//				return defaultAssembly;

//			try
//			{
//				// TODO  chang to PluginAssemblyLoadContext.LoadFromAssemblyPath(path)
//				// return loadContext.LoadFromAssemblyPath(path);

//				return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
//			}
//			catch (Exception)
//			{
//				return null;
//			}
//		}

//		private WidgetConfigDescritpion LoadDescriptionFromJsonFile(DirectoryInfo directory)
//		{
//			var file = Path.Combine(directory.FullName, "plugin.json");
//			if (!File.Exists(file))
//				return null;

//			try
//			{
//				var content = File.ReadAllText(file);

//				return JsonSerializer.Deserialize<WidgetConfigDescritpion>(content, JsonHelper.Options);
//			}
//			catch (Exception)
//			{
//			}

//			return null;
//		}


//		public class WidgetConfigDescritpion
//		{
//			public string Id { get; set; }
//			public string Name { get; set; }
//			public string Description { get; set; }
//			public string Author { get; set; }
//			public string Version { get; set; }
//		}
//	}
//}
