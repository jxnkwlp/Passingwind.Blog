using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Extensions;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class CommentService : Service<Comment>, ICommentService
	{
		public CommentService(IRepository<Comment, int> Repository) : base(Repository)
		{
		}

		public async Task<IList<Comment>> GetCommentsByPostId(int postId, bool onlyPublished)
		{
			var query = GetQueryable();

			if (onlyPublished)
				query = query.Where(t => t.IsApproved && !t.IsDeleted && !t.IsSpam);

			var result = await query.Where(t => t.PostId.Equals(postId)).ToListAsync();

			return result;
		}

		public async Task<bool> IsTrustUserAsync(string email)
		{
			var result = await GetQueryable().Where(t => t.Email.ToLower() == email.ToLower() && t.IsApproved && !t.IsSpam).AnyAsync();

			return result;
		}

		public async Task SetIsDeletedAsync(int id)
		{
			var entity = await GetByIdAsync(id);

			if (entity != null)
			{
				if (!entity.IsDeleted)
				{
					entity.IsDeleted = true;

					await UpdateAsync(entity);
				}
			}
		}

		public async Task UpdateIsApprovedAsync(int id, bool value)
		{
			var entity = await GetByIdAsync(id);

			if (entity != null)
			{
				if (entity.IsApproved != value)
				{
					entity.IsApproved = value;

					await UpdateAsync(entity);
				}
			}
		}

		public async Task UpdateIsSpamAsync(int id, bool value)
		{
			var entity = await GetByIdAsync(id);

			if (entity != null)
			{
				if (entity.IsSpam != value)
				{
					entity.IsSpam = true;

					await UpdateAsync(entity);
				}
			}
		}

		public IQueryable<Comment> GetQueryable()
		{
			return Repository.GetQueryable().OrderByDescending(t => t.CreationTime);
		}

		public async Task<IPagedList<Comment>> GetCommentsPagedListAsync(CommentListInputModel input)
		{
			var query = Repository.GetQueryable();

			if (input.IncludeOptions?.IncludePosts == true)
				query = query.Include(t => t.Post); 

			query = query
						.WhereIf(t => t.Content.Contains(input.SearchTerm), () => !string.IsNullOrEmpty(input.SearchTerm))
						.WhereIf(t => t.Author == input.Author, () => !string.IsNullOrEmpty(input.Author))
						.WhereIf(t => t.Email == input.Email, () => !string.IsNullOrEmpty(input.Email))
						.WhereIf(t => t.PostId == input.PostId, () => input.PostId.HasValue)
						.WhereIf(t => t.IsApproved == input.Approved, () => input.Approved.HasValue)
						.WhereIf(t => t.IsSpam == input.Spam, () => input.Spam.HasValue)
						;

			// orders
			if (input.Orders?.Count > 0)
			{
				foreach (var order in input.Orders)
				{
					query = query.OrderBy($"{order.Key} {order.Value}");
				}
			}
			else
			{
				query = query.OrderByDescending(t => t.CreationTime);
			}

			return await query.ToPagedListAsync(input);
		}

		public async Task<Comment> GetByGuidAsync(Guid id)
		{
			return await Repository.GetQueryable().FirstOrDefaultAsync(t => t.GuidId == id);
		}
	}
}
