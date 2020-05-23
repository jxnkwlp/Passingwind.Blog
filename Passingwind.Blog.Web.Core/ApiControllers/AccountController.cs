using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Services;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using Passingwind.Blog.Web.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class AccountController : ApiControllerBase
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly BlogUserManager _userManager;
		private readonly BlogRoleManager _roleManager;
		private readonly IUserFactory _userFactory;
		private readonly BlogSignInManager _signInManager;

		public AccountController(IHttpContextAccessor httpContextAccessor, BlogUserManager userManager, BlogRoleManager roleManager, IUserFactory userFactory, BlogSignInManager signInManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
			_roleManager = roleManager;
			_userFactory = userFactory;
			_signInManager = signInManager;
		}

		[HttpPost("profile")]
		public async Task UpdateProfileAsync([FromBody] UserProfileUpdateModel model)
		{
			var cp = _httpContextAccessor.HttpContext.User;

			var user = await _userManager.FindByNameAsync(cp.Identity.Name);

			_userFactory.ToEntity(model, user);

			await _userManager.UpdateAsync(user);
		}

		[HttpGet("login")]
		public async Task<IEnumerable<UserLoginInfo>> GetLogins()
		{
			var cp = _httpContextAccessor.HttpContext.User;
			var user = await _userManager.GetUserAsync(cp);

			if (user == null)
				return null;

			return await _userManager.GetLoginsAsync(user);
		}

		[HttpGet("ExternalAuthenticationSchemes")]
		public async Task<object> GetExternalAuthenticationSchemesAsync()
		{
			var logins = await _signInManager.GetExternalAuthenticationSchemesAsync();

			return logins.Select(t => new { t.Name, t.DisplayName });
		}

		[HttpPost("removelogin")]
		public async Task RemoveLoginAsync([FromBody] RemoveLoginRequest request)
		{
			var cp = _httpContextAccessor.HttpContext.User;
			var user = await _userManager.GetUserAsync(cp);

			var result = await _userManager.RemoveLoginAsync(user, request.LoginProvider, request.ProviderKey);

			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
			}
			else
			{
				throw new System.Exception(string.Join(",", result.Errors.Select(t => t.Description)));
			}
		}

		public class RemoveLoginRequest
		{
			[Required]
			public string LoginProvider { get; set; }
			[Required]
			public string ProviderKey { get; set; }
		}
	}
}
