using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Data
{
	public class DbContextFactory<TDbContext> : IDbContextFactory<TDbContext>
	{
		public DbContextFactory()
		{

		}

		public IDbContext GetDbContext()
		{
			throw new NotImplementedException();
		}

		public DbSet<TEntity> GetEntities<TEntity>() where TEntity : class
		{
			throw new NotImplementedException();
		}
	}
}
