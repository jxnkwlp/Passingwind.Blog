using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Data
{
	public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
	{ }

	public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
	{
		Task InsertAsync(TEntity entity, bool save = true, CancellationToken cancellationToken = default);

		Task UpdateAsync(TEntity entity, bool save = true, CancellationToken cancellationToken = default);

		Task DeleteAsync(TEntity entity, bool save = true, CancellationToken cancellationToken = default);

		Task DeleteByIdAsync(TKey key, bool save = true, CancellationToken cancellationToken = default);

		Task DeleteByWhereAsync(Expression<Func<TEntity, bool>> predicate, bool save = true, CancellationToken cancellationToken = default);

		Task<TEntity> GetByIdAsync(TKey key, CancellationToken cancellationToken = default);

		Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);

		Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);

		Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
		Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);

		IQueryable<TEntity> Includes(params Expression<Func<TEntity, object>>[] predicates);

		Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyName, CancellationToken cancellationToken = default) where TProperty : class;

		Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyName, CancellationToken cancellationToken = default) where TProperty : class;

		Task<IQueryable<TProperty>> LoadCollectionQueryAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyName, CancellationToken cancellationToken = default) where TProperty : class;

		Task UpdateCollectionAsync<TProperty, TPropertyKey>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, IEnumerable<TProperty> newCollections, Func<TProperty, TPropertyKey> idResolve, bool save = true, CancellationToken cancellationToken = default) where TProperty : class;

		IQueryable<TEntity> GetQueryable();
	}
}
