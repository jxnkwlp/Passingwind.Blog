using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.DTO;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class TagsController : ApiControllerBase
	{
		private readonly ITagsService _tagsService;
		private readonly ITagsFactory _tagsFactory;

		public TagsController(ITagsService tagsService, ITagsFactory tagsFactory)
		{
			_tagsService = tagsService;
			_tagsFactory = tagsFactory;
		}

		[ApiPermission("tags.list", "post.edit", Condition = ApiPermissionMultipleCondition.Or)]
		[HttpGet]
		public async Task<ApiPagedListOutput<TagsModel>> GetListAsync([FromQuery] TagsApiListQueryModel model)
		{
			var limit = model.Limit;
			var skip = model.Skip;

			if (!model.AllowPage)
			{
				limit = int.MaxValue;
				skip = 0;
			}
			var list = await _tagsService.GetTagsPagedListAsync(new TagsListInputModel()
			{
				Limit = limit,
				Skip = skip,
				SearchTerm = model.SearchTerm,
			});

			return new ApiPagedListOutput<TagsModel>(list.Count, list.Select(t => _tagsFactory.ToModel(t, new TagsModel())).ToList());
		}

		[ApiPermission("tags.create")]
		[HttpPost]
		public async Task<TagsModel> PostAsync([FromBody]TagsEditModel model)
		{
			var entity = await _tagsService.GetOrCreateAsync(model.Name);

			return _tagsFactory.ToModel(entity, new TagsModel());
		}

		[ApiPermission("tags.delete")]
		[HttpDelete]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task DeleteAsync([FromBody] string[] names)
		{
			if (names != null && names.Any())
				await _tagsService.DeleteByAsync(t => names.Contains(t.Name));
		}
	}
}
