using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Services;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class IdentityController : ApiControllerBase
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly BlogUserManager _userManager;
		private readonly BlogRoleManager _roleManager;
		private readonly IUserFactory _userFactory;

		public IdentityController(IHttpContextAccessor httpContextAccessor, BlogUserManager userManager, BlogRoleManager roleManager, IUserFactory userFactory)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
			_roleManager = roleManager;
			_userFactory = userFactory;
		}

		[HttpGet]
		public async Task<CurrentUserProfileModel> GetCurrentUserProfleAsync()
		{
			var cp = _httpContextAccessor.HttpContext.User;

			var user = await _userManager.FindByNameAsync(cp.Identity.Name);
			var roles = await _userManager.GetRolesAsync(user);
			var permissions = await _roleManager.GetRolesPermissionsAsync(roles.ToArray());

			var model = new CurrentUserProfileModel()
			{
				DisplayName = user.DisplayName,
				PhoneNumber = user.PhoneNumber,
				Bio = user.Bio,
				UserDescription = user.UserDescription,
				Email = user.Email,

				Roles = roles,
				Permissions = permissions,

				AvatarUrl = AvatarHelper.GetSrc(user.Email),
			};

#if DEBUG
			model.IdentityName = cp.Identity.Name;
			model.AuthenticationType = cp.Identity.AuthenticationType;
			model.Claims = cp.Claims.Select(t => new IdentityClaim(t.Type, t.Value)).ToArray();
#endif

			return model;
		}


	}
}
