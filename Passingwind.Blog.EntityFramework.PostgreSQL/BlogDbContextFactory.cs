using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Passingwind.Blog.Data;
using System.IO;

namespace Passingwind.Blog.EntityFramework.PostgreSQL
{
	public class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
	{
		public BlogDbContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
			  .SetBasePath(Directory.GetCurrentDirectory())
			  .AddJsonFile("appsettings.json")
			  .Build();

			var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("Default"), b =>
			{
				b.MigrationsAssembly(typeof(BlogDbContextFactory).Assembly.FullName);
			});

			return new BlogDbContext(optionsBuilder.Options);
		}
	}
}
