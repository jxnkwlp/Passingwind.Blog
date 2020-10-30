using Passingwind.Blog.Data;
using Passingwind.PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Repositories
{
	public interface IReadOnlyRepository<TEntity, TKey> where TEntity : IEntity<TKey>
	{
		Task<long> GetCountAsync(CancellationToken cancellationToken = default);

		Task<IList<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default);

		Task<IPagedList> GetPagedListAsync(int maxResultCount = int.MaxValue, int skipCount = 0, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default);

		Task<TEntity> GetAsync(TKey key, bool includeDetails = true, CancellationToken cancellationToken = default);
		Task<TEntity> FindAsync(TKey key, bool includeDetails = true, CancellationToken cancellationToken = default);

		IQueryable<TEntity> GetQueryable(bool includeDetails = true);
	}
}
