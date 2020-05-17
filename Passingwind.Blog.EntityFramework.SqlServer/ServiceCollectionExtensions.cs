using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.EntityFramework.SqlServer
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddSqlServerDbContext(this IServiceCollection services, DbConnectionString dbConnectionString)
		{
			services.AddDbContext<BlogDbContext>(options =>
			{
				options.UseSqlServer(dbConnectionString.ConnectionString, b =>
				{
					b.EnableRetryOnFailure(3);
					b.MigrationsAssembly(typeof(BlogDbContextFactory).Assembly.FullName);
				});
			});

			return services;
		}
	}
}
