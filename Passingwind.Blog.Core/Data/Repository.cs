using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Data
{
	public class Repository<TEntity> : Repository<TEntity, int> where TEntity : class, IEntity<int>
	{
		public Repository(DbContext dbContext) : base(dbContext)
		{
		}
	}

	public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
	{
		protected readonly DbContext DbContext;
		protected readonly DbSet<TEntity> Entities;

		public Repository(DbContext dbContext)
		{
			DbContext = dbContext;
			Entities = dbContext.Set<TEntity>();
		}

		public virtual IQueryable<TEntity> GetQueryable() => Entities.Cacheable();

		public virtual async Task DeleteAsync(TEntity entity, bool save = true, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			Entities.Remove(entity);

			if (save) await SaveChangesAsync(cancellationToken);
		}

		public virtual async Task DeleteByIdAsync(TKey key, bool save = true, CancellationToken cancellationToken = default)
		{
			var entity = await Entities.FindAsync(key);

			if (entity != null)
				await DeleteAsync(entity, save, cancellationToken);
		}

		public virtual async Task DeleteByWhereAsync(Expression<Func<TEntity, bool>> predicate, bool save = true, CancellationToken cancellationToken = default)
		{
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));

			var list = await Entities.Where(predicate).ToListAsync(cancellationToken);

			Entities.RemoveRange(list);

			if (save) await SaveChangesAsync(cancellationToken);
		}

		public virtual async Task<TEntity> GetByIdAsync(TKey key, CancellationToken cancellationToken = default)
		{
			return await Entities.FindAsync(keyValues: new object[] { key }, cancellationToken: cancellationToken);
		}

		public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			if (predicate != null)
				return await GetQueryable().FirstOrDefaultAsync(predicate, cancellationToken);

			return await GetQueryable().FirstOrDefaultAsync(cancellationToken);
		}

		public virtual async Task<IList<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
		{
			return await GetQueryable().ToListAsync(cancellationToken);
		}

		public virtual async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			if (predicate != null)
				return await GetQueryable().Where(predicate).ToListAsync(cancellationToken);
			return await GetQueryable().ToListAsync(cancellationToken);
		}

		public virtual IQueryable<TEntity> Includes(params Expression<Func<TEntity, object>>[] predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));

			var query = GetQueryable().AsQueryable();
			foreach (var item in predicate)
			{
				query = query.Include(item);
			}

			return query;
		}

		public virtual async Task InsertAsync(TEntity entity, bool save = true, CancellationToken cancellationToken = default)
		{
			await DbContext.AddAsync(entity);

			if (save)
				await SaveChangesAsync(cancellationToken);
		}

		public virtual async Task UpdateAsync(TEntity entity, bool save = true, CancellationToken cancellationToken = default)
		{
			// TODO 
			if (DbContext.Entry(entity).State == EntityState.Detached)
			{
				var attached = Entities.Local.FirstOrDefault(t => t.Id.Equals(entity.Id));
				if (attached == null)
					DbContext.Update(entity);
				// throw new Exception();

				else
				{
					DbContext.Entry(attached).CurrentValues.SetValues(entity);
				}
			}
			else
			{
				DbContext.Update(entity);
			}

			if (save)
				await SaveChangesAsync(cancellationToken);
		}

		public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			var query = GetQueryable().AsQueryable();
			if (predicate != null)
				query = query.Where(predicate);

			return await query.CountAsync(cancellationToken);
		}

		public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			var query = GetQueryable().AsQueryable();
			if (predicate != null)
				query = query.Where(predicate);

			return await query.LongCountAsync(cancellationToken);
		}


		public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyName, CancellationToken cancellationToken = default) where TProperty : class
		{
			var entry = DbContext.Entry(entity);

			if (entry.State == EntityState.Detached)
				await entry.ReloadAsync();

			await entry
						.Collection(propertyName)
						.LoadAsync(cancellationToken);
		}

		public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyName, CancellationToken cancellationToken = default) where TProperty : class
		{
			var entry = DbContext.Entry(entity);

			if (entry.State == EntityState.Detached)
				await entry.ReloadAsync();

			await entry
						.Reference(propertyName)
						.LoadAsync(cancellationToken);
		}

		public virtual async Task<IQueryable<TProperty>> LoadCollectionQueryAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyName, CancellationToken cancellationToken = default) where TProperty : class
		{
			var entry = DbContext.Entry(entity);

			if (entry.State == EntityState.Detached)
				await entry.ReloadAsync();

			return entry
						  .Collection(propertyName)
						  .Query();
		}


		protected virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await DbContext.SaveChangesAsync(cancellationToken);
		}

	}
}
