using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class PostController : ApiControllerBase
	{
		private readonly IPostService _postService;
		private readonly IPostFactory _postFactory;

		private readonly ICategoryService _categoryService;
		private readonly ITagsService _tagsService;

		public PostController(IPostService postService,
						IPostFactory postFactory,
						ICategoryService categoryService,
						ITagsService tagsService)
		{
			_postService = postService;
			_postFactory = postFactory;
			_categoryService = categoryService;
			_tagsService = tagsService;
		}

		protected Task<IEnumerable<PostCategory>> GetPostCategoriesAsync(Post post, PostEditModel model)
		{
			if (model.Categories?.Any() == true)
			{
				var categoryIds = model.Categories;
				var newList = categoryIds.Select(t => new PostCategory()
				{
					CategoryId = t,
					PostId = post.Id,
				});

				return Task.FromResult(newList);
			}

			return Task.FromResult(Enumerable.Empty<PostCategory>());
		}

		protected async Task<IEnumerable<PostTags>> GetPostTagsAsync(Post post, PostEditModel model)
		{
			if (model.Tags?.Any() == true)
			{
				var tags = new List<Tags>();
				foreach (var item in model.Tags)
				{
					var tag = await _tagsService.GetOrCreateAsync(item);
					tags.Add(tag);
				}

				var newList = tags.Select(t => new PostTags()
				{
					TagsId = t.Id,
					PostId = post.Id,
				});

				return newList;
			}

			return Enumerable.Empty<PostTags>();
		}

		[HttpGet("{id}")]
		public async Task<PostModel> Get(int id, [FromQuery] PostApiGetQueryModel input)
		{
			var post = await _postService.GetByIdAsync(id, new PostIncludeOptions()
			{
				IncludeCategory = input.IncludeCategory,
				IncludeTags = input.IncludeTags,
				IncludeUser = input.IncludeUser,
			});

			if (post == null)
				return null;

			var model = _postFactory.ToModel(post, new PostModel());

			return model;
		}

		[ApiPermission("post.list")]
		[HttpGet]
		public async Task<ApiPagedListOutput<PostModel>> GetListAsync([FromQuery]PostApiListQueryModel model)
		{
			var list = await _postService.GetPostsPagedListAsync(new PostListInputModel()
			{
				Limit = model.Limit,
				Skip = model.Skip,
				SearchTerm = model.SearchTerm,
				UserId = model.UserId,
				CategoryId = model.CategoryId,
				IsDraft = model.IsDraft,
				IncludeOptions = new PostIncludeOptions()
				{
					IncludeCategory = model.IncludeCategory,
					IncludeTags = model.IncludeTags,
					IncludeUser = model.IncludeUser,
				},
				Orders = model.Orders?.Where(t => t.Field != null).ToDictionary(t => t.Field, t => t.Order),
			});

			return new ApiPagedListOutput<PostModel>(list.TotalCount, list.Select(t => _postFactory.ToModel(t, new PostModel())).ToList());
		}

		[ApiPermission("post.edit")]
		[HttpPost]
		public async Task<PostEditModel> EditAsync([FromBody]PostEditModel model)
		{
			if (model.Id > 0)
			{
				var entity = await _postService.GetByIdAsync(model.Id, new PostIncludeOptions()
				{
					IncludeTags = true,
					IncludeCategory = true,
				});

				if (entity == null)
					return null;

				entity = _postFactory.ToEntity(model, entity);

				var categories = await GetPostCategoriesAsync(entity, model);
				var tags = await GetPostTagsAsync(entity, model);

				await _postService.UpdateAsync(entity, categories, tags);
			}
			else
			{
				var entity = _postFactory.ToEntity(model);

				var categories = await GetPostCategoriesAsync(entity, model);
				var tags = await GetPostTagsAsync(entity, model);
				entity.Categories = categories.ToList();
				entity.Tags = tags.ToList();

				await _postService.InsertAsync(entity);

				model.Id = entity.Id;
			}

			return model;
		}

		[ApiPermission("post.delete")]
		[HttpDelete]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task DeleteAsync([FromBody] int[] ids)
		{
			if (ids != null && ids.Any())
				await _postService.DeleteByAsync(t => ids.Contains(t.Id));
		}

		[ApiPermission("post.published")]
		[HttpPost("Published")]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task SetPublished([FromBody] PostApiUpdateIsPublishedModel model)
		{
			if (model.Ids?.Any() == true)
			{
				foreach (var item in model.Ids)
				{
					await _postService.UpdateIsPublishAsync(item, model.Value);
				}
			}
		}
	}
}
