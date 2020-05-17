using Passingwind.PagedList;
using System.Collections.Generic;

namespace Passingwind.Blog.Web.Models
{
	public class ApiPagedListOutput<T>
	{
		public int Count { get; }

		public IEnumerable<T> Value { get; }

		public ApiPagedListOutput(int count, IEnumerable<T> data)
		{
			Count = count;
			Value = data;
		}

		public ApiPagedListOutput(IPagedList<T> list)
		{
			Count = list.TotalCount;
			Value = list;
		}
	}
}
