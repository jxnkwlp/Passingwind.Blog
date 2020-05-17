using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Passingwind.Blog.Services
{
	public interface IPostService : IService<Post>
	{
		Task<Post> FindBySlugAsync(string slug, PostIncludeOptions includeOptions = null, CancellationToken cancellationToken = default);

		Task<Post> GetByIdAsync(int id, PostIncludeOptions includeOptions = null, CancellationToken cancellationToken = default);

		Task UpdateIsPublishAsync(int postId, bool published, CancellationToken cancellationToken = default);

		Task<int> IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default);

		Task<int> IncreaseCommentsCountAsync(int id);

		Task<IPagedList<Post>> GetPostsPagedListAsync(PostListInputModel input, CancellationToken cancellationToken = default);

		Task<IEnumerable<Post>> GetPostListAsync(PostListInputModel input, CancellationToken cancellationToken = default);

		Task<SortedDictionary<DateTime, int>> GetCountsByPublishYearAndMonthAsync();
	}
}
