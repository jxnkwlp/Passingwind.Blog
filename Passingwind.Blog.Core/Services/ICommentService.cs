using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface ICommentService : IService<Comment>, IScopedDependency
	{
		public Task<Comment> GetByGuidAsync(Guid id);
		IQueryable<Comment> GetQueryable();
		Task<IList<Comment>> GetCommentsByPostId(int postId, bool onlyPublished);
		Task<bool> IsTrustUserAsync(string email);
		Task SetIsDeletedAsync(int id);
		Task UpdateIsApprovedAsync(int id, bool value);
		Task UpdateIsSpamAsync(int id, bool value);

		Task<IPagedList<Comment>> GetCommentsPagedListAsync(CommentListInputModel input);
	}
}
