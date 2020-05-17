using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Utils;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class RoleController : ApiControllerBase
	{
		private readonly BlogRoleManager _roleManager;
		private readonly IRoleFactory _roleFactory;

		public RoleController(BlogRoleManager roleManager, IRoleFactory roleFactory)
		{
			_roleManager = roleManager;
			_roleFactory = roleFactory;
		}

		[ApiPermission("role.list", "user.list", "post.edit", Condition = ApiPermissionMultipleCondition.Or)]
		[HttpGet]
		public async Task<ApiPagedListOutput<RoleModel>> GetListAsync([FromQuery] RoleApiListQueryModel model)
		{
			var list = await _roleManager.GetRolesPagedListAsync(new RolePagedListInputModel()
			{
				SearchTerm = model.SearchTerm,
				Limit = model.Limit,
				Skip = model.Skip,
				IncludePermissionKeys = model.IncludePermissionKeys,
			});

			return new ApiPagedListOutput<RoleModel>(list.TotalCount, list.Select(t => _roleFactory.ToModel(t, new RoleModel())).ToList());
		}

		[HttpGet("{id}")]
		public async Task<RoleModel> GetAsync([FromRoute] string id)
		{
			var entity = await _roleManager.FindByIdAsync(id);

			if (entity == null)
				return null;

			return _roleFactory.ToModel(entity, new RoleModel());
		}

		[ApiPermission("role.create")]
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task CreateAsync([FromBody] RoleModel model)
		{
			var entity = _roleFactory.ToEntity(model);
			entity.Id = GuidGenerator.Instance.Create().ToString();
			entity.Permissions = model.PermissionKeys?.Select(t => new RolePermission { Key = t, }).ToList();

			var result = await _roleManager.CreateAsync(entity);

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}
		}

		[ApiPermission("role.update")]
		[HttpPut]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task UpdateAsync([FromBody] RoleModel model)
		{
			if (string.IsNullOrEmpty(model.Id))
				throw new ArgumentNullException(nameof(model.Id), "Id can't be empty.");

			var entity = await _roleManager.FindByIdAsync(model.Id);
			if (entity == null)
				throw new Exception("The role can't found.");

			entity = _roleFactory.ToEntity(model, entity);

			var result = await _roleManager.UpdateAsync(entity);

			await _roleManager.UpdatePermissionsAsync(entity.Id, model.PermissionKeys);

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}
		}

		[ApiPermission("role.delete")]
		[HttpDelete]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task DeleteAsync([FromBody] string[] ids)
		{
			if (ids != null && ids.Any())
				foreach (var item in ids)
				{
					await _roleManager.DeleteByIdAsync(item);
				}
		}
	}
}
