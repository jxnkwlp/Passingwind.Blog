using Passingwind.Blog.Data;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Repositories
{
	public interface IRepository<TEntity, TKey> : IBasicRepository<TEntity, TKey> where TEntity : IEntity<TKey>
	{
		Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default);

		Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default);

		Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool save = false, CancellationToken cancellationToken = default);

	}
}
