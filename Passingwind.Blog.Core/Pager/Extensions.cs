using System;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog.Pager
{
    public static class Extensions
    {
        public static IPagedList<T2> ToPagedList<T1, T2>(this IQueryable<T1> query, int pageNumber, int pageSize, Func<IPagedList<T1>, IList<T2>> action)
        {
            var list1 = new PagedList<T1>(query, pageNumber, pageSize);

            var result = action(list1);

            return new PagedList<T2>(result.ToList(), list1.PageNumber, list1.PageSize, list1.TotalItems);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return new PagedList<T>(query, pageNumber, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IList<T> source, int pageNumber, int pageSize, int totalItems)
        {
            return new PagedList<T>(source, pageNumber, pageSize, totalItems);
        }
    }
}
