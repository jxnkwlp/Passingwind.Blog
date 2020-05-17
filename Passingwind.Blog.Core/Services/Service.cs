using Passingwind.Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IService<TEntity> : IService<TEntity, int> where TEntity : class, IEntity<int> { }

	public interface IService<TEntity, TKey> where TEntity : class, IEntity<TKey>
	{
		Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
		Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
		Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
		Task DeleteByIdAsync(TKey key, CancellationToken cancellationToken = default);
		Task DeleteByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
		Task<TEntity> GetByIdAsync(TKey key, CancellationToken cancellationToken = default);
		Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
	}

	public class Service<TEntity> : Service<TEntity, int> where TEntity : class, IEntity<int>
	{
		public Service(IRepository<TEntity, int> repository) : base(repository)
		{
		}
	}

	public class Service<TEntity, TKey> : IService<TEntity, TKey> where TEntity : class, IEntity<TKey>
	{
		protected IRepository<TEntity, TKey> Repository { get; }

		public Service(IRepository<TEntity, TKey> Repository)
		{
			this.Repository = Repository;
		}

		public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			await Repository.DeleteAsync(entity, true, cancellationToken);
		}

		public virtual async Task DeleteByIdAsync(TKey key, CancellationToken cancellationToken = default)
		{
			await Repository.DeleteByIdAsync(key, true, cancellationToken);
		}

		public async Task DeleteByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
		{
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));

			await Repository.DeleteByWhereAsync(predicate);
		}

		public virtual async Task<TEntity> GetByIdAsync(TKey key, CancellationToken cancellationToken = default)
		{
			return await Repository.GetByIdAsync(key, cancellationToken);
		}

		public virtual async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			return await Repository.GetListAsync(predicate, cancellationToken);
		}

		public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			await Repository.InsertAsync(entity, true, cancellationToken);
		}

		public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			await Repository.UpdateAsync(entity, true, cancellationToken);
		}

	}
}
