using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Data;

namespace Passingwind.Blog.EntityFramework.MySql
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMySqlDbContext(this IServiceCollection services, DbConnectionString dbConnectionString)
		{
			services.AddDbContext<BlogDbContext>(options =>
			{
				options.UseMySql(dbConnectionString.ConnectionString, b =>
								{
									b.CharSet(Pomelo.EntityFrameworkCore.MySql.Storage.CharSet.Utf8Mb4);
									b.EnableRetryOnFailure(3);
									b.MigrationsAssembly(typeof(BlogDbContextFactory).Assembly.FullName);
								});
			});

			return services;
		}
	}
}
