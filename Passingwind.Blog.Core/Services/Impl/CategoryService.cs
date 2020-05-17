using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class CategoryService : Service<Category>, ICategoryService
	{
		private readonly ISlugService _slugService;

		public CategoryService(IRepository<Category, int> Repository, ISlugService slugService) : base(Repository)
		{
			_slugService = slugService;
		}

		public override async Task InsertAsync(Category entity, CancellationToken cancellationToken = default)
		{
			entity.Slug = await _slugService.NormalarAsync(entity.Slug);
			await base.InsertAsync(entity, cancellationToken);
		}

		public override async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
		{
			entity.Slug = await _slugService.NormalarAsync(entity.Slug);
			await base.UpdateAsync(entity, cancellationToken);
		}

		public async Task<Category> GetBySlugAsync(string slug)
		{
			if (slug == null)
				throw new ArgumentNullException(nameof(slug));

#pragma warning disable RCS1155 // Use StringComparison when comparing strings.
			return await Repository.FirstOrDefaultAsync(t => t.Slug.ToUpper() == slug.ToUpper());
#pragma warning restore RCS1155 // Use StringComparison when comparing strings.
		}

		public async Task<int> GetPostCountAsync(int categoryId, bool includeDraft = true)
		{
			var entity = await GetByIdAsync(categoryId);
			if (entity == null)
				return default;

			var query = await Repository.LoadCollectionQueryAsync(entity, t => t.Posts);
			if (!includeDraft)
			{
				query = query.Where(t => !t.Post.IsDraft);
			}

			var count = await query.CountAsync();

			return count;
		}

		public IQueryable<Category> GetQueryable()
		{
			return Repository.Includes(t => t.Posts).OrderBy(t => t.DisplayOrder).ThenBy(t => t.Name);
		}

		public override async Task<IList<Category>> GetListAsync(Expression<Func<Category, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			if (predicate != null)
				return await GetQueryable().Where(predicate).ToListAsync(cancellationToken);
			else
				return await GetQueryable().ToListAsync(cancellationToken);
		}

	}
}
