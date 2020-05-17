using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Passingwind.Blog.Extensions
{
	public static class DbContextOptionsBuilderExtensions
	{
		public static DbContextOptionsBuilder UseOptions(this DbContextOptionsBuilder builder, DbContextOptions options)
		{
			if (options == null)
				throw new System.ArgumentNullException(nameof(options));

			foreach (var item in options.Extensions)
			{
				if (item is Microsoft.EntityFrameworkCore.Infrastructure.CoreOptionsExtension)
					continue;

				((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(item);
			}

			return builder;
		}
	}
}
