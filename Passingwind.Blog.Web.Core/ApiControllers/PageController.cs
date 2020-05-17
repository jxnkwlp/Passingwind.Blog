using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class PageController : ApiControllerBase
	{
		private readonly IPageService _pageService;
		private readonly IPageFactory _pageFactory;

		public PageController(IPageService pageService, IPageFactory pageFactory)
		{
			_pageService = pageService;
			_pageFactory = pageFactory;
		}

		[ApiPermission("page.list")]
		[HttpGet]
		public async Task<ApiPagedListOutput<PageModel>> GetListAsync([FromQuery] ApiListQueryModel model)
		{
			var list = await _pageService.GetPagesPagedListAsync(new ListBasicQueryInput()
			{
				SearchTerm = model.SearchTerm,
				Limit = model.Limit,
				Skip = model.Skip
			});

			return new ApiPagedListOutput<PageModel>(list.TotalCount, list.Select(t => _pageFactory.ToModel(t, new PageModel())).ToList());
		}


		[HttpGet("{id}")]
		public async Task<PageModel> GetAsync([FromRoute] int id)
		{
			var entity = await _pageService.GetByIdAsync(id);

			if (entity == null)
				return null;

			return _pageFactory.ToModel(entity, new PageModel());
		}

		[ApiPermission("page.create")]
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task<PageModel> CreateAsync([FromBody] PageModel model)
		{
			var entity = _pageFactory.ToEntity(model, new Page());

			await _pageService.InsertAsync(entity);

			model.Id = entity.Id;
			return model;
		}

		[ApiPermission("page.update")]
		[HttpPut]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task<PageModel> UpdateAsync([FromBody] PageModel model)
		{
			if (model.Id == 0)
				throw new ArgumentNullException(nameof(model.Id), "Id must be than zero.");

			var entity = _pageFactory.ToEntity(model, new Page());

			await _pageService.UpdateAsync(entity);

			return model;
		}

		[ApiPermission("page.delete")]
		[HttpDelete]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task DeleteAsync([FromBody] int[] ids)
		{
			if (ids != null && ids.Any())
				await _pageService.DeleteByAsync(t => ids.Contains(t.Id));
		}

	}
}
