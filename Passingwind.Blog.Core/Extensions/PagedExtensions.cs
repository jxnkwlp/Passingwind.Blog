using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Models;
using Passingwind.PagedList;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Extensions
{
	public static class PagedExtensions
	{
		/// <summary>
		///  for ef query
		/// </summary> 
		public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, IQueryOffsetInput model, CancellationToken cancellationToken = default)
		{
			var count = await source.CountAsync();
			var list = await source.Skip(model.Skip).Take(model.Limit).ToListAsync(cancellationToken);

			var result = new PagedList<T>();
			result.LoadSource(list, model.GetPageNumber(), model.Limit, count);
			return result;
		}
	}
}
