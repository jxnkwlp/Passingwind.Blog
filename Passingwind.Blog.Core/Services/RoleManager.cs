using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Extensions;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public class BlogRoleManager : RoleManager<Role>, IScopedDependency
	{
		private readonly IRepository<Role, string> _repository;

		public BlogRoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger, IRepository<Role, string> repository) : base(store, roleValidators, keyNormalizer, errors, logger)
		{
			_repository = repository;
		}

		public virtual IQueryable<Role> GetQueryable()
		{
			return Roles;
		}

		public virtual Task<IPagedList<Role>> GetRolesPagedListAsync(RolePagedListInputModel input)
		{
			var query = GetQueryable();

			if (input.IncludePermissionKeys)
				query = query.Include(t => t.Permissions);

			query = query.WhereIf(t => t.Name.Contains(input.SearchTerm), () => !string.IsNullOrEmpty(input.SearchTerm));

			return query.OrderBy(t => t.Name).ToPagedListAsync(input);
		}

		public async Task DeleteByIdAsync(string id)
		{
			var entity = await _repository.FirstOrDefaultAsync(t => t.Id == id);
			if (entity != null)
			{
				await DeleteAsync(entity);
			}
		}

		public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName)
		{
			var normalizedName = NormalizeKey(roleName);
			var role = await FindByNameAsync(roleName);

			if (role == null)
				throw new Exception($"The role '{roleName}' not found.");

			await _repository.LoadCollectionAsync(role, t => t.Permissions);

			return role.Permissions.Select(t => t.Key).ToArray();
		}

		public async Task<IEnumerable<string>> GetRolesPermissionsAsync(params string[] roleName)
		{
			var result = new List<string>();
			foreach (var item in roleName)
			{
				result.AddRange(await GetRolePermissionsAsync(item));
			}

			return result.Distinct().ToArray();
		}

		public async Task UpdatePermissionsAsync(string roleId, IEnumerable<string> keys)
		{
			var role = await _repository.Includes(t => t.Permissions).FirstOrDefaultAsync(t => t.Id == roleId);

			if (role == null)
				throw new Exception("The role not found.");


			var rolePermissionList = keys?.Select(t => new RolePermission()
			{
				Key = t.Trim(),
				RoleId = role.Id
			}) ?? Enumerable.Empty<RolePermission>();

			await _repository.UpdateCollectionAsync(role, t => t.Permissions, rolePermissionList, t => t.Key);
		}
	}
}
