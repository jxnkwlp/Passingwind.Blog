using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog.Pager
{
	public static class Extensions
	{
		public static IPagedList<T2> ToPagedList<T1, T2>(this IQueryable<T1> query, int pageNumber, int pageSize, Func<IPagedList<T1>, IList<T2>> action)
		{
			var list1 = query.ToPagedList(pageNumber, pageSize);

			var list2 = new PagedList<T2>();

			list2.LoadSource(action(list1), list1.PageNumber, list1.PageSize, list1.TotalCount);

			return list2;
		}
	}
}
