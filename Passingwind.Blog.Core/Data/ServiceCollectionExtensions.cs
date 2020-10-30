using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Blog.Data
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDbContext<TDbContext>(IServiceCollection services) where TDbContext : IDbContext
		{


			return services;
		}
	}
}
