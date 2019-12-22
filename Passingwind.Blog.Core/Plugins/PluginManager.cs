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

		private static IList<PluginDescriptor> _pluginDescriptions = null;

		public PluginManager(IPluginLoader pluginLoader, IServiceProvider serviceProvider)
		{
			_pluginLoader = pluginLoader;
			_serviceProvider = serviceProvider;
		}

		protected void LoadPlugs()
		{
			var list = _pluginLoader.Load() ?? Enumerable.Empty<PluginPackage>();

			foreach (var item in list)
			{
				LoadPluginDescription(item);
			}
		}

		//private void AddPluginToPart(PluginPackage pluginPackage, ApplicationPartManager applicationPartManager)
		//{
		//	var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginPackage.Assembly);

		// var applicationParts = partFactory.GetApplicationParts(pluginPackage.Assembly);

		// foreach (var part in applicationParts) {
		// applicationPartManager.ApplicationParts.Add(part); }

		// var relatedAssemblies =
		// RelatedAssemblyAttribute.GetRelatedAssemblies(pluginPackage.Assembly, true);

		//	//foreach (var assembly in relatedAssemblies)
		//	//{
		//	//	partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
		//	//	foreach (var part in partFactory.GetApplicationParts(assembly))
		//	//	{
		//	//		applicationPartManager.ApplicationParts.Add(part);
		//	//	}
		//	//}
		//}

		private void LoadPluginDescription(PluginPackage pluginPackage)
		{
			if (_pluginDescriptions == null)
				_pluginDescriptions = new List<PluginDescriptor>();

			var description = GetPluginDescription(pluginPackage);

			_pluginDescriptions.Add(description);
		}

		public IEnumerable<PluginDescriptor> GetAllPlugins()
		{
			if (_pluginDescriptions == null)
			{
				LoadPlugs();
			}

			return _pluginDescriptions ?? Enumerable.Empty<PluginDescriptor>();
		}

		public PluginDescriptor GetPluginDescription(string name)
		{
			if (_pluginDescriptions != null)
				return _pluginDescriptions.FirstOrDefault(t => t.Name == name);
			return null;
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
				AssemblyName = pluginPackage.AssemblyName,
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
				pluginDescription.Group = description.Group;
			}

			return pluginDescription;
		}
	}
}
