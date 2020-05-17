using Microsoft.AspNetCore.Identity;
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
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class UserController : ApiControllerBase
	{
		private readonly BlogUserManager _userManager;
		private readonly IUserFactory _userFactory;
		private readonly BlogRoleManager _roleManager;
		private readonly IRoleFactory _roleFactory;

		public UserController(BlogUserManager userManager, IUserFactory userFactory, IRoleFactory roleFactory, BlogRoleManager roleManager)
		{
			_userManager = userManager;
			_userFactory = userFactory;
			_roleFactory = roleFactory;
			_roleManager = roleManager;
		}

		[ApiPermission("user.list")]
		[HttpGet]
		public async Task<ApiPagedListOutput<UserModel>> GetListAsync([FromQuery]  UserApiPagedListQueryModel model)
		{
			var limit = model.Limit;
			var skip = model.Skip;

			var list = await _userManager.GetUserPagedListAsync(new UserPagedListInputModel()
			{
				Limit = model.Limit,
				Skip = model.Skip,
				SearchTerm = model.SearchTerm,
				IncludeRoles = model.IncludeRoles,
				IncludeRolePermissionKeys = false,
				Orders = model.Orders?.Where(t => t.Field != null).ToDictionary(t => t.Field, t => t.Order),
			});

			return new ApiPagedListOutput<UserModel>(list.Count, list.Select(t =>
			{
				var m = _userFactory.ToModel(t, new UserModel());
				m.Roles = t.UserRoles?.Select(t => _roleFactory.ToModel(t.Role, new RoleModel()));

				return m;
			}).ToList());
		}

		[ApiPermission("user.create")]
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task CreateAsync([FromBody] UserEditModel model)
		{
			var entity = _userFactory.ToEntity(model);
			entity.Id = GuidGenerator.Instance.Create().ToString();

			IdentityResult result;
			if (!string.IsNullOrEmpty(model.Password))
				result = await _userManager.CreateAsync(entity, model.Password);
			else
				result = await _userManager.CreateAsync(entity);

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}

			if (model.RoleIds?.Any() == true)
			{
				var roles = new List<Role>();
				foreach (var item in model.RoleIds)
				{
					var role = await _roleManager.FindByIdAsync(item);
					if (role != null)
						roles.Add(role);
				}
				if (roles.Any())
					await _userManager.AddToRolesAsync(entity, roles.Select(t => t.Name));
			}
		}

		[ApiPermission("user.update")]
		[HttpPut]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task UpdateAsync([FromBody] UserEditModel model)
		{
			if (string.IsNullOrEmpty(model.Id))
				throw new ArgumentNullException(nameof(model.Id), "Id must be required.");

			var entity = await _userManager.FindByIdAsync(model.Id);
			if (entity == null)
				throw new Exception("User not found.");

			entity = _userFactory.ToEntity(model, entity);

			var result = await _userManager.UpdateAsync(entity);

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}

			if (!string.IsNullOrEmpty(model.Password))
			{
				result = await _userManager.FocusResetPassowrdAsync(entity, model.Password);
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}
			}

			var userExistsRoleNames = await _userManager.GetRolesAsync(entity);

			if (model.RoleIds?.Any() == true)
			{
				var newRoles = new List<Role>();
				foreach (var item in model.RoleIds)
				{
					var role = await _roleManager.FindByIdAsync(item);
					if (role != null)
					{
						newRoles.Add(role);

						//if (!(await _userManager.IsInRoleAsync(entity, role.Name)))
						//{
						//	await _userManager.AddToRoleAsync(entity, role.Name);
						//}
					}
				}

				var needRemove = userExistsRoleNames.Except(newRoles.Select(t => t.Name));
				var needAdd = newRoles.Select(t => t.Name).Except(userExistsRoleNames);

				if (needRemove.Any())
					await _userManager.RemoveFromRolesAsync(entity, needRemove);

				if (needAdd.Any())
					await _userManager.AddToRolesAsync(entity, needAdd);
			}
			else
			{
				if (userExistsRoleNames.Any())
				{
					await _userManager.RemoveFromRolesAsync(entity, userExistsRoleNames);
				}
			}
		}

		[ApiPermission("user.delete")]
		[HttpDelete]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task DeleteAsync([FromBody] string[] ids)
		{
			if (ids != null && ids.Any())
				foreach (var item in ids)
				{
					await _userManager.DeleteByIdAsync(item);
				}
		}

		[ApiPermission("user.setlock")]
		[HttpPost("lock")]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task SetLockUserAsync([FromBody] UserSetLockInputModel model)
		{
			if (model.UserIds?.Any() == true)
				foreach (var item in model.UserIds)
				{
					var user = await _userManager.FindByIdAsync(item);
					if (user != null)
						await _userManager.SetLockoutEndDateAsync(user, model.LockEnd);
				}
		}
	}
}
