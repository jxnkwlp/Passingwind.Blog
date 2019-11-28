using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Blog.Plugins
{
	public interface IPluginManager
	{
		IEnumerable<PluginDescriptor> GetAllPluginDescription(string name);
		IPlugin GetPlugin(string name);
		PluginDescriptor GetPluginDescription(string name);
		void RegisterPlugins(IServiceCollection services, ApplicationPartManager applicationPartManager);
	}
}