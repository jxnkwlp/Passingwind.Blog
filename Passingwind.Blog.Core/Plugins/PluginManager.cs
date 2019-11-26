using Microsoft.AspNetCore.Mvc.ApplicationParts;
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

		public void LoadPlugins(ApplicationPartManager applicationPartManager)
		{
			var list = _pluginLoader.Load();

			VerifyPackages(list);


			foreach (var item in list)
			{
				var partFactory = ApplicationPartFactory.GetApplicationPartFactory(item.Assembly);

				var applicationParts = partFactory.GetApplicationParts(item.Assembly);

				foreach (var part in applicationParts)
				{
					applicationPartManager.ApplicationParts.Add(part);
				}

				var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(item.Assembly, true);

				//foreach (var assembly in relatedAssemblies)
				//{
				//	partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
				//	foreach (var part in partFactory.GetApplicationParts(assembly))
				//	{
				//		applicationPartManager.ApplicationParts.Add(part);
				//	}
				//}
			}
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

		protected void VerifyPackages(IEnumerable<PluginPackage> plugins)
		{
			foreach (var item in plugins)
			{
				var describeFile = Path.Combine(item.ContentPath, "plugin.json");

				if (File.Exists(describeFile))
				{
					var module = JsonSerializer.Deserialize<PluginDescription>(File.ReadAllText(describeFile));

					_pluginDescriptions.Add(module);
					_plugins[module.Name] = (IPlugin)Activator.CreateInstance(item.PluginType);
				}
				else
				{
					_pluginDescriptions.Add(new PluginDescription() { Name = item.Assembly.FullName, });
				}


			}

		}

	}
}
