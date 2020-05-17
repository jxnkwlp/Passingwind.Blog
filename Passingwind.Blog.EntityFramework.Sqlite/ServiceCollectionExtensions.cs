using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Data;

namespace Passingwind.Blog.EntityFramework.Sqlite
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddSqliteDbContext(this IServiceCollection services, DbConnectionString dbConnectionString)
		{
			services.AddDbContext<BlogDbContext>(options =>
			{
				options.UseSqlite(dbConnectionString.ConnectionString, b =>
				{
					b.MigrationsAssembly(typeof(BlogDbContextFactory).Assembly.FullName);
				});
			});

			return services;
		}
	}
}
