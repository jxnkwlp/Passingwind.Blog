using System.Collections.Generic;

namespace Passingwind.Blog.Plugins
{
	public interface IPluginManager
	{
		IEnumerable<PluginDescriptor> GetAllPlugins();

		IPlugin GetPluginInstance(string name);

		PluginDescriptor GetPluginDescription(string name);

	}
}