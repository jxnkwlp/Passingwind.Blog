using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Passingwind.Blog.Web.Themes
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddThemes(this IServiceCollection services, IHostEnvironment hostEnvironment)
		{
			var factory = new ThemeFactory(services, hostEnvironment);
			return services;
		}
	}
}
