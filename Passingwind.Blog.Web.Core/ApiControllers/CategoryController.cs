using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services;
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
	public class CategoryController : ApiControllerBase
	{
		private readonly ICategoryService _categoryService;
		private readonly ICategoryFactory _categoryFactory;

		public CategoryController(ICategoryService categoryService, ICategoryFactory categoryFactory)
		{
			_categoryService = categoryService;
			_categoryFactory = categoryFactory;
		}

		[ApiPermission("category.list", "post.edit", Condition = ApiPermissionMultipleCondition.Or)]
		[HttpGet]
		public async Task<IEnumerable<CategoryListItemModel>> GetListAsync()
		{
			var list = await _categoryService.GetListAsync();

			return list.Select(t => _categoryFactory.ToModel(t, new CategoryListItemModel())).ToList();
		}

		[ApiPermission("category.create")]
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		//[ProducesResponseType(StatusCodes.Status201Created)]
		//[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task CreateAsync([FromBody] CategoryModel model)
		{
			var entity = _categoryFactory.ToEntity(model, new Category());

			await _categoryService.InsertAsync(entity);
		}

		[ApiPermission("category.update")]
		[HttpPut]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task UpdateAsync([FromBody] CategoryModel model)
		{
			if (model.Id == 0)
				throw new ArgumentNullException(nameof(model.Id), "Id must be than zero.");

			var entity = _categoryFactory.ToEntity(model, new Category());

			await _categoryService.UpdateAsync(entity);
		}

		[ApiPermission("category.delete")]
		[HttpDelete]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task DeleteAsync([FromBody] int[] ids)
		{
			if (ids != null && ids.Any())
				await _categoryService.DeleteByAsync(t => ids.Contains(t.Id));
		}

	}
}
