using System.Collections.Generic;

namespace Passingwind.Blog.Plugins
{
	public interface IPluginManager
	{
		IEnumerable<PluginDescriptor> GetAllPlugins();

		IPlugin GetPlugin(string name);

		PluginDescriptor GetPluginDescription(string name);

	}
}