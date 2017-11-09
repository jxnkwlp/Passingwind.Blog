using System.Collections.Generic;

namespace Passingwind.Blog.Pager
{
    /// <summary>
    ///  Defines a pager data list
    /// </summary>
    public interface IPagedList
    {
        int PageNumber { get; set; }

        int PageSize { get; set; }

        int TotalPages { get; }

        int TotalItems { get; }

    }

    public interface IPagedList<T> : IPagedList, IList<T>
    {
    }
}
