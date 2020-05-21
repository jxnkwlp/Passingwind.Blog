using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Data
{
	internal static class Extensions
	{
		public static async Task TryUpdateManyToManyAsync<T, TKey>(this DbContext db, IEnumerable<T> currentItems, IEnumerable<T> newItems, Func<T, TKey> getKeyFunc) where T : class
		{
			var removeList = currentItems.Except(newItems, getKeyFunc).ToArray();
			var addList = newItems.Except(currentItems, getKeyFunc).ToArray();
			db.Set<T>().RemoveRange(removeList);
			await db.Set<T>().AddRangeAsync(addList);
		}

		public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKeyFunc)
		{
			return items
				.GroupJoin(other, getKeyFunc, getKeyFunc, (item, tempItems) => new { item, tempItems })
				.SelectMany(t => t.tempItems.DefaultIfEmpty(), (t, temp) => new { t, temp })
				.Where(t => ReferenceEquals(null, t.temp) || t.temp.Equals(default(T)))
				.Select(t => t.t.item);
		}
	}
}
