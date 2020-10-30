using Passingwind.Blog.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Repositories
{
	public interface IBasicRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey> where TEntity : IEntity<TKey>
	{
		Task<TEntity> InsertAsync(TEntity entity, bool save = false, CancellationToken cancellationToken = default);
		Task<TEntity> UpdateAsync(TEntity entity, bool save = false, CancellationToken cancellationToken = default);
		Task DeleteAsync(TEntity entity, bool save = false, CancellationToken cancellationToken = default);
		Task DeleteAsync(TKey key, bool save = false, CancellationToken cancellationToken = default);
	}
}
