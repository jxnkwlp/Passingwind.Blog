using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Data;

namespace Passingwind.Blog.EntityFramework.PostgreSQL
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPostgreSQLDbContext(this IServiceCollection services, DbConnectionString dbConnectionString)
		{
			services.AddDbContext<BlogDbContext>(options =>
			{
				options.UseNpgsql(dbConnectionString.ConnectionString, b =>
				{
					b.MigrationsAssembly(typeof(BlogDbContextFactory).Assembly.FullName);
					b.EnableRetryOnFailure(3); 
				});
			});

			return services;
		}
	}
}
