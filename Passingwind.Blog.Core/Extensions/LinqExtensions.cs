using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Extensions
{
	public static class LinqExtensions
	{
		public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, Func<bool> condition)
		{
			if (query is null)
			{
				throw new ArgumentNullException(nameof(query));
			}

			if (predicate is null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			if (condition is null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			if (condition())
			{
				return query.Where(predicate);
			}

			return query;
		}
	}
}
