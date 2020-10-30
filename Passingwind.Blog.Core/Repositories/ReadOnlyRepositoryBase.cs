using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Repositories
{
	public abstract class ReadOnlyRepositoryBase<TDbContext, TEntity, TKey> : IReadOnlyRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
	{
		protected IDbContextFactory DbContextFactory { get; }
		protected DbSet<TEntity> Entities { get; }

		public ReadOnlyRepositoryBase(IDbContextFactory dbContextFactory)
		{
			DbContextFactory = dbContextFactory;
			this.Entities = dbContextFactory.GetEntities<TEntity>();
		}


		public Task<TEntity> FindAsync(TKey key, bool includeDetails = true, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<TEntity> GetAsync(TKey key, bool includeDetails = true, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<long> GetCountAsync(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IList<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IPagedList> GetPagedListAsync(int maxResultCount = int.MaxValue, int skipCount = 0, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IQueryable<TEntity> GetQueryable(bool includeDetails = true)
		{
			throw new NotImplementedException();
		}
	}
}
