using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Blog.Repositories
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRepository<TDbContext, TEntity>(IServiceCollection services)
		{
			// TODO

			return services;
		}
	}
}
