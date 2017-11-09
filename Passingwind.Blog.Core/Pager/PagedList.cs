using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Pager
{
    public class PagedList<T> : List<T>, IPagedList<T>, IPagedList
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; private set; }

        public int TotalPages { get; private set; }



        public PagedList(IList<T> source, int pageNumber, int pageSize, int totalItems)
        {
            if (pageSize <= 0) pageSize = 1;
            if (pageNumber < 1) pageNumber = 1;

            this.PageNumber = pageNumber;
            this.PageSize = pageSize;

            this.AddRange(source);

            CalculationTotalPages(totalItems);
        }

        public PagedList(IEnumerable<T> source, int pageNumber, int pageSize, int totalItems) :
            this(source.ToList(), pageNumber, pageSize, totalItems)
        {
        }

        public PagedList(IQueryable<T> source, int pageNumber, int pageSize, int totalItems) :
            this(source.ToList(), pageNumber, pageSize, totalItems)
        {
        }

        public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageSize <= 0) pageSize = 1;
            if (pageNumber < 1) pageNumber = 1;

            this.PageNumber = pageNumber;
            this.PageSize = pageSize;

            var result = source.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            this.AddRange(result);

            CalculationTotalPages(source.Count());
        }

        private void CalculationTotalPages(int totalItems)
        {
            this.TotalItems = totalItems;

            TotalPages = totalItems % PageSize == 0 ?
                totalItems / PageSize :
                (totalItems / PageSize + 1);

            if (TotalPages <= 1)
                TotalPages = 1;
            if (PageNumber > TotalPages)
                PageNumber = TotalPages;
        }
    }
}
