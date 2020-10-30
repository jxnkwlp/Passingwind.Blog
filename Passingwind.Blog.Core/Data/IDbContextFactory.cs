using Microsoft.EntityFrameworkCore;

namespace Passingwind.Blog.Data
{
	public interface IDbContextFactory<TDbContext>
	{
		IDbContext GetDbContext();
		DbSet<TEntity> GetEntities<TEntity>() where TEntity : class;
	}
}
