using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Blog.Plugins
{
	public abstract class PluginBase : IPlugin
	{
		public virtual void PostConfigureServices(IServiceCollection services)
		{
		}
	}
}
