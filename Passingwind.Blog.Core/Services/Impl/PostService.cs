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
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class PostService : Service<Post>, IPostService
	{
		private readonly IPostRepository _repository;
		private readonly ISlugService _slugService;

		public PostService(IPostRepository repository, ISlugService slugService) : base(repository)
		{
			_repository = repository;
			_slugService = slugService;
		}

		private IQueryable<Post> BuildPostListInputQuery(IQueryable<Post> query, PostListInputModel input)
		{
			// include options
			if (input.IncludeOptions?.IncludeUser == true)
				query = query.Include(t => t.User);
			if (input.IncludeOptions?.IncludeTags == true)
				query = query.Include(t => t.Tags).ThenInclude(t => t.Tags);
			if (input.IncludeOptions?.IncludeCategory == true)
				query = query.Include(t => t.Categories).ThenInclude(c => c.Category);

			// where filter options
			query = query
						.WhereIf(t => t.Title.Contains(input.SearchTerm), () => !string.IsNullOrWhiteSpace(input.SearchTerm))
						.WhereIf(t => t.UserId == input.UserId, () => !string.IsNullOrWhiteSpace(input.UserId))
						.WhereIf(t => t.IsDraft == input.IsDraft, () => input.IsDraft.HasValue)
						.WhereIf(t => t.Categories.Any(c => c.CategoryId == input.CategoryId), () => input.CategoryId.HasValue)
						.WhereIf(t => t.Tags.Any(c => c.TagsId == input.TagsId), () => input.TagsId.HasValue)
						.WhereIf(t => t.PublishedTime.Year == input.PublishedYearMonth.Value.Year && t.PublishedTime.Month == input.PublishedYearMonth.Value.Month, () => input.PublishedYearMonth.HasValue)
						.WhereIf(t => t.PublishedTime.Date == input.PublishedDate, () => input.PublishedDate.HasValue)
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
				query = query.OrderByDescending(t => t.PublishedTime)
									.ThenByDescending(t => t.CreationTime);
			}

			return query;
		}

		public override async Task InsertAsync(Post entity, CancellationToken cancellationToken = default)
		{
			entity.Slug = await _slugService.NormalarAsync(entity.Slug);
			await base.InsertAsync(entity, cancellationToken);
		}

		public override async Task UpdateAsync(Post entity, CancellationToken cancellationToken = default)
		{
			entity.Slug = await _slugService.NormalarAsync(entity.Slug);
			await base.UpdateAsync(entity, cancellationToken);
		}

		public async Task<Post> GetByIdAsync(int id, PostIncludeOptions includeOptions = null, CancellationToken cancellationToken = default)
		{
			var query = Repository.GetQueryable();

			if (includeOptions?.IncludeCategory == true)
				query = query.Include(t => t.Categories).ThenInclude(c => c.Category);

			if (includeOptions?.IncludeTags == true)
				query = query.Include(t => t.Tags).ThenInclude(t => t.Tags);

			if (includeOptions?.IncludeUser == true)
				query = query.Include(t => t.User);

			return await query.FirstOrDefaultAsync(t => t.Id == id);
		}

		public async Task<Post> FindBySlugAsync(string slug, PostIncludeOptions includeOptions = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(slug))
			{
				throw new ArgumentNullException(nameof(slug));
			}
			// BUG 20191013
			// https://github.com/aspnet/EntityFrameworkCore/issues/18020
			// https://github.com/aspnet/EntityFrameworkCore/issues/1222
			// https://github.com/aspnet/EntityFrameworkCore/issues/18234
			// String equals with StringComparison.InvariantCultureIgnoreCase can't work !

			var query = Repository.GetQueryable();

			if (includeOptions?.IncludeCategory == true)
				query = query.Include(t => t.Categories).ThenInclude(c => c.Category);

			if (includeOptions?.IncludeTags == true)
				query = query.Include(t => t.Tags).ThenInclude(t => t.Tags);

			if (includeOptions?.IncludeUser == true)
				query = query.Include(t => t.User);

			return await query.FirstOrDefaultAsync(t => t.Slug.ToLower() == slug.ToLower());
		}

		public async Task UpdateIsPublishAsync(int postId, bool published, CancellationToken cancellationToken = default)
		{
			var post = await _repository.GetByIdAsync(postId, cancellationToken);
			if (post != null)
			{
				post.IsDraft = !published;

				await _repository.UpdateAsync(post, true, cancellationToken);
			}
		}

		public async Task<int> IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default)
		{
			var post = await _repository.GetByIdAsync(postId);
			if (post != null)
				post.ViewsCount++;

			await UpdateAsync(post);

			return post.CommentsCount;
		}

		public Task<IPagedList<Post>> GetPostsPagedListAsync(PostListInputModel input, CancellationToken cancellationToken = default)
		{
			var query = Repository.GetQueryable();

			query = BuildPostListInputQuery(query, input);

			// return 
			return query.ToPagedListAsync(input);
		}

		public async Task<int> IncreaseCommentsCountAsync(int id)
		{
			var post = await GetByIdAsync(id);
			if (post == null)
				throw new Exception($"The post '{id}' not found.");

			post.CommentsCount++;

			await UpdateAsync(post);

			return post.CommentsCount;
		}

		public async Task<IEnumerable<Post>> GetPostListAsync(PostListInputModel input, CancellationToken cancellationToken = default)
		{
			var query = Repository.GetQueryable();

			query = BuildPostListInputQuery(query, input);

			return await query.ToListAsync(cancellationToken);
		}

		public async Task<SortedDictionary<DateTime, int>> GetCountsByPublishYearAndMonthAsync()
		{
			var query = await Repository.GetQueryable()
										.Where(t => t.IsDraft == false)
										.GroupBy(t => new { t.PublishedTime.Date.Year, t.PublishedTime.Date.Month })
										.Select(t => new { t.Key, Count = t.Count() }).ToListAsync();

			var result = new SortedDictionary<DateTime, int>();

			foreach (var item in query)
			{
				result[new DateTime(item.Key.Year, item.Key.Month, 1)] = item.Count;
			}

			return result;
		}

		public async Task UpdateCategoriesAsync(Post post, IEnumerable<PostCategory> postCategories, bool saveChanges = true)
		{
			await _repository.UpdateCollectionAsync(post, t => t.Categories, postCategories, x => x.CategoryId, saveChanges);
		}

		public async Task UpdateTagsAsync(Post post, IEnumerable<PostTags> postTags, bool saveChanges = true)
		{
			await _repository.UpdateCollectionAsync(post, t => t.Tags, postTags, x => x.TagsId, saveChanges);
		}



		//#region Category

		//public Task<IList<PostCategory>> GetPostCategoriesAsync(int postId)
		//{
		//	return Task.FromResult<IList<PostCategory>>(_store.GetQueryable<PostCategory>().Where(t => t.PostId == postId).ToList());
		//}

		//public Task<IList<Category>> GetCategoriesAsync(int postId)
		//{
		//	var categoryIds = _store.GetQueryable<PostCategory>().Where(t => t.PostId == postId).Select(t => t.CategoryId);

		//	return Task.FromResult<IList<Category>>(_store.GetQueryable<Category>().Where(t => categoryIds.Contains(t.Id)).ToList());
		//}

		//public async Task RemoveCategoryAsync(Post post, string categoryId)
		//{
		//	var postCategory = await _store.FindByAsync<PostCategory>(t => t.CategoryId == categoryId && t.PostId == post.Id);
		//	if (postCategory != null)
		//	{
		//		await _store.DeleteAsync(postCategory);
		//	}
		//}

		//public async Task RemoveCategoryAsync(PostCategory postCategory)
		//{
		//	if (postCategory != null)
		//	{
		//		await _store.DeleteAsync(postCategory);
		//	}
		//}

		//#endregion Category

		//#region tags

		//public Task<IList<string>> GetTagsStringListAsync(int postId)
		//{
		//	var tagsIds = _store.GetQueryable<PostTags>().Where(t => t.PostId == postId).Select(t => t.TagsId).ToList();

		//	var query = _store.GetQueryable<Tags>().Where(t => tagsIds.Contains(t.Id)).Select(t => t.Name).ToList();

		//	return Task.FromResult<IList<string>>(query);
		//}

		//public Task<IList<PostTags>> GetTagsAsync(int postId)
		//{
		//	return Task.FromResult<IList<PostTags>>(_store.GetQueryable<PostTags>().Where(t => t.PostId == postId).ToList());
		//}

		//public async Task RemoveTagsAsync(Post post, string tagId)
		//{
		//	var postTags = await _store.FindByAsync<PostTags>(t => t.TagsId == tagId && t.PostId == post.Id);
		//	if (postTags != null)
		//	{
		//		await _store.DeleteAsync(postTags);
		//	}
		//}

		//#endregion tags

		//#region Comments

		//public async Task<int> IncreaseCommentsCountAsync(int postId)
		//{
		//	var post = await FindByIdAsync(postId);
		//	if (post != null)
		//		post.CommentsCount++;

		//	await UpdateAsync(post);

		//	return post.CommentsCount;
		//}

		//public async Task<int> ReduceCommentsCountAsync(int postId)
		//{
		//	var post = await FindByIdAsync(postId);
		//	if (post != null && post.CommentsCount > 0)
		//		post.CommentsCount--;

		//	await UpdateAsync(post);

		//	return post.CommentsCount;
		//}

		//#endregion Comments

	}
}
