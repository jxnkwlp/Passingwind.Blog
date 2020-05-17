using Passingwind.Blog.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Extensions
{
	public static class Extensions
	{
		public static int GetPageNumber(this IQueryOffsetInput model)
		{
			return model.Skip / model.Limit + 1;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action?.Invoke(item);
			}
		}

	}
}
