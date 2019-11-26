using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Passingwind.Blog.Plugins
{
	public interface IPluginManager
	{
		IEnumerable<PluginDescription> GetAllPluginDescription(string name);
		IPlugin GetPlugin(string name);
		PluginDescription GetPluginDescription(string name);
		void LoadPlugins(ApplicationPartManager applicationPartManager);
	}
}