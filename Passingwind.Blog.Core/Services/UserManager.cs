using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Extensions;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Passingwind.Blog.Services
{
	public class BlogUserManager : Microsoft.AspNetCore.Identity.UserManager<User>
	{
		public BlogUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}

		//public async Task<bool> VerifyPasswordAsync(User user, string password)
		//{
		//	var verifyResult = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

		//	return await Task.FromResult(verifyResult == PasswordVerificationResult.Success);
		//}

		public async Task<IdentityResult> FocusResetPassowrdAsync(User user, string password)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException(nameof(password));
			}

			// var verifyResult = await this.ValidatePasswordAsync(user, password); 
			return await UpdatePasswordHash(user, password, true);
		}

		public async Task RemoveFromAllRolesAsync(User user)
		{
			var allRoles = await GetRolesAsync(user);
			if (allRoles != null)
			{
				await RemoveFromRolesAsync(user, allRoles);
			}
		}

		public IQueryable<User> GetQueryable()
		{
			return Users;
		}

		public async Task<IEnumerable<User>> GetUserListAsync(UserPagedListInputModel input)
		{
			var query = Users;

			if (input.IncludeRoles && input.IncludeRolePermissionKeys)
				query = query.Include(t => t.UserRoles).ThenInclude(t => t.Role).ThenInclude(t => t.Permissions);
			else if (input.IncludeRoles)
				query = query.Include(t => t.UserRoles).ThenInclude(t => t.Role);

			query = query.WhereIf(t => t.UserName.Contains(input.SearchTerm) || t.Email.Contains(input.SearchTerm) || t.DisplayName.Contains(input.SearchTerm), () => !string.IsNullOrEmpty(input.SearchTerm));

			// orders
			if (input.Orders?.Count > 0)
			{
				foreach (var order in (input.Orders))
				{
					query = query.OrderBy($"{order.Key} {order.Value}");
				}
			}
			else
			{
				query = query.OrderByDescending(t => t.CreationTime);
			}

			return await query.ToListAsync();
		}

		public async Task<IPagedList<User>> GetUserPagedListAsync(UserPagedListInputModel input)
		{
			var query = Users;

			if (input.IncludeRoles && input.IncludeRolePermissionKeys)
				query = query.Include(t => t.UserRoles).ThenInclude(t => t.Role).ThenInclude(t => t.Permissions);
			else if (input.IncludeRoles)
				query = query.Include(t => t.UserRoles).ThenInclude(t => t.Role);

			query = query.WhereIf(t => t.UserName.Contains(input.SearchTerm) || t.Email.Contains(input.SearchTerm) || t.DisplayName.Contains(input.SearchTerm), () => !string.IsNullOrEmpty(input.SearchTerm));

			// orders
			if (input.Orders?.Count > 0)
			{
				foreach (var order in (input.Orders))
				{
					query = query.OrderBy($"{order.Key} {order.Value}");
				}
			}
			else
			{
				query = query.OrderByDescending(t => t.CreationTime);
			}

			return await query.ToPagedListAsync(input);
		}

		public async Task DeleteByIdAsync(string id)
		{
			var entity = await Users.FirstOrDefaultAsync(t => t.Id == id);
			if (entity != null)
				await DeleteAsync(entity);
		}
	}
}
