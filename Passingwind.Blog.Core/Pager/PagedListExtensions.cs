using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Pager
{
    public static class PagedListExtensions
    {
        public static bool HasFirst(this IPagedList pagedList)
        {
            return pagedList.PageNumber > 1;
        }

        public static bool HasPrevious(this IPagedList pagedList)
        {
            return pagedList.PageNumber > 1;
        }

        public static bool HasNext(this IPagedList pagedList)
        {
            return pagedList.PageNumber < pagedList.TotalPages;
        }

        public static bool HasLast(this IPagedList pagedList)
        {
            return pagedList.PageNumber < pagedList.TotalPages;
        }

        public static int GetPreviousNumber(this IPagedList pagedList)
        {
            var i = pagedList.PageNumber - 1;

            return i <= 1 ? 1 : i;
        }

        public static int GetNextNumber(this IPagedList pagedList)
        {
            var i = pagedList.PageNumber + 1;

            return i >= pagedList.TotalPages ? pagedList.TotalPages : i;
        }


        public static int GetStartNumber(this IPagedList pagedList, int itemCount = 5)
        {
            var s = itemCount / 2;

            var index = pagedList.PageNumber - s;

            if (pagedList.TotalPages - itemCount < index)
                index = pagedList.TotalPages - itemCount + 1;

            if (index <= 1)
                index = 1;

            return index;
        }

        public static int GetEndNumber(this IPagedList pagedList, int itemCount = 5)
        {
            var s = itemCount / 2;

            var index = pagedList.PageNumber + s;

            if (index <= itemCount)
                index = itemCount;

            if (index >= pagedList.TotalPages)
                index = pagedList.TotalPages;

            return index;
        }


        public static bool HasNextMore(this IPagedList pagedList, int itemCount = 5)
        {
            var end = GetEndNumber(pagedList, itemCount);

            return (end < pagedList.TotalPages);
        }

        public static bool HasPreviousMore(this IPagedList pagedList, int itemCount = 5)
        {
            var start = GetStartNumber(pagedList, itemCount);

            return (start > 1);
        }
    }
}
