using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Blog.Plugins
{
	public interface IPlugin
	{
		void PostConfigureServices(IServiceCollection services);

	}
}
