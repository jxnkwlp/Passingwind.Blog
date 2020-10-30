using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Blog
{
	public interface IStartup
	{
		void ConfigureServices(IServiceCollection services);
	}
}
