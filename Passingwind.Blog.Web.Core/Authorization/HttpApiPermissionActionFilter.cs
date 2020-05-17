using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services;
using Passingwind.Blog.Web.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Authorization
{
	public class HttpApiPermissionActionFilter : IAsyncAuthorizationFilter, IOrderedFilter
	{
		private readonly BlogRoleManager _roleManager;
		private readonly BlogUserManager _userManager;

		public HttpApiPermissionActionFilter(BlogRoleManager roleManager, BlogUserManager userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public int Order => -1;

		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			if (!context.HttpContext.User.Identity.IsAuthenticated)
				return;

			if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				var actionMethodInfo = controllerActionDescriptor.MethodInfo;
				if (actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null || controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null)
					return;

				var apiPermissionAttribute = actionMethodInfo.GetCustomAttribute<ApiPermissionAttribute>();
				if (apiPermissionAttribute == null)
				{
					apiPermissionAttribute = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<ApiPermissionAttribute>();
				}

				if (apiPermissionAttribute == null)
					return;

				var requiredKeys = apiPermissionAttribute.Keys;

				if (requiredKeys?.Any() == false)
					return;

				await CheckUserAsync(context, apiPermissionAttribute.Condition, requiredKeys);
			}

		}

		private async Task CheckUserAsync(AuthorizationFilterContext context, ApiPermissionMultipleCondition multipleCondition, params string[] requiredKeys)
		{
			var userId = context.HttpContext.User.GetUserId();

			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			var roleNames = await _userManager.GetRolesAsync(user);

			if (!roleNames.Any())
			{
				context.Result = new ForbidResult();
				return;
			}

			if (roleNames.Any(t => t == Role.AdministratorName))
			{
				return;
			}

			var permissions = new List<string>();

			foreach (var roleName in roleNames)
			{
				var rolePermissions = await _roleManager.GetRolePermissionsAsync(roleName);
				permissions.AddRange(rolePermissions);
			}

			permissions = permissions.Distinct().ToList();

			if (!permissions.Any())
			{
				context.Result = new ForbidResult();
				return;
			}

			if (multipleCondition == ApiPermissionMultipleCondition.And && permissions.Intersect(requiredKeys).Count() != requiredKeys.Count())
				context.Result = new ForbidResult();
			else if (multipleCondition == ApiPermissionMultipleCondition.Or && permissions.Intersect(requiredKeys).Count() == 0)
			{
				context.Result = new ForbidResult();
			}
		}
	}
}
