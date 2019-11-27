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

		private readonly Dictionary<string, IPlugin> _plugins = new Dictionary<string, IPlugin>();
		private readonly IList<PluginDescription> _pluginDescriptions = new List<PluginDescription>();

		public PluginManager(IPluginLoader pluginLoader)
		{
			_pluginLoader = pluginLoader;
		}

		//public IEnumerable<AssemblyPart> GetAssemblyParts()
		//{
		//	var list = _pluginLoader.Load();

		//	return null;
		//}

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

		public IEnumerable<PluginDescription> GetAllPluginDescription(string name)
		{
			return _pluginDescriptions;
		}

		public PluginDescription GetPluginDescription(string name)
		{
			return _pluginDescriptions.FirstOrDefault(t => t.Name == name);
		}

		public IPlugin GetPlugin(string name)
		{
			if (_plugins.ContainsKey(name))
				return _plugins[name];
			return null;
		}

		private PluginDescription GetPluginDescription(PluginPackage pluginPackage)
		{
			var describeFile = Path.Combine(pluginPackage.ContentPath, "plugin.json");

			if (File.Exists(describeFile))
			{
				var description = JsonSerializer.Deserialize<PluginDescription>(File.ReadAllText(describeFile));

				return description;
			}
			else
			{
				return new PluginDescription() { Name = pluginPackage.Assembly.FullName, };
			}
		}

	}
}
