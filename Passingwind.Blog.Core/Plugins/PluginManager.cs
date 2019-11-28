using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Passingwind.Blog.Plugins
{
	public class PluginManager : IPluginManager
	{
		private readonly IPluginLoader _pluginLoader;
		private readonly IServiceProvider _serviceProvider;

		private static readonly IList<PluginDescriptor> _pluginDescriptions = new List<PluginDescriptor>();

		public PluginManager(IPluginLoader pluginLoader, IServiceProvider serviceProvider)
		{
			_pluginLoader = pluginLoader;
			_serviceProvider = serviceProvider;
		}

		public void RegisterPlugins(IServiceCollection services, ApplicationPartManager applicationPartManager)
		{
			var list = _pluginLoader.Load();

			foreach (var item in list)
			{
				AddPluginServices(item, services);
				AddPluginToPart(item, applicationPartManager);
			}
		}

		private void AddPluginToPart(PluginPackage pluginPackage, ApplicationPartManager applicationPartManager)
		{
			var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginPackage.Assembly);

			var applicationParts = partFactory.GetApplicationParts(pluginPackage.Assembly);

			foreach (var part in applicationParts)
			{
				applicationPartManager.ApplicationParts.Add(part);
			}

			var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(pluginPackage.Assembly, true);

			//foreach (var assembly in relatedAssemblies)
			//{
			//	partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
			//	foreach (var part in partFactory.GetApplicationParts(assembly))
			//	{
			//		applicationPartManager.ApplicationParts.Add(part);
			//	}
			//} 
		}

		private void AddPluginServices(PluginPackage pluginPackage, IServiceCollection services)
		{
			services.AddScoped(pluginPackage.PluginType);

			var description = GetPluginDescription(pluginPackage);

			_pluginDescriptions.Add(description);
		}

		public IEnumerable<PluginDescriptor> GetAllPluginDescription(string name)
		{
			return _pluginDescriptions;
		}

		public PluginDescriptor GetPluginDescription(string name)
		{
			return _pluginDescriptions.FirstOrDefault(t => t.Name == name);
		}

		public IPlugin GetPlugin(string name)
		{
			var plugin = GetPluginDescription(name);
			if (plugin == null)
				return null;

			return _serviceProvider.CreateScope().ServiceProvider.GetService(plugin.PluginType) as IPlugin;
		}

		private PluginDescriptor GetPluginDescription(PluginPackage pluginPackage)
		{
			var describeFile = Path.Combine(pluginPackage.ContentPath, "plugin.json");

			PluginDescriptor pluginDescription = new PluginDescriptor()
			{
				Assembly = pluginPackage.Assembly,
				ContentPath = pluginPackage.ContentPath,
				RelativePath = pluginPackage.RelativePath,
				PluginType = pluginPackage.PluginType,
				Name = pluginPackage.Assembly.FullName,
			};

			if (File.Exists(describeFile))
			{
				var description = JsonSerializer.Deserialize<PluginDescriptor>(File.ReadAllText(describeFile));

				pluginDescription.Author = description.Author;
				pluginDescription.Name = description.Name;
				pluginDescription.Version = description.Version;
				pluginDescription.Description = description.Description;
			}


			return pluginDescription;
		}

	}
}
