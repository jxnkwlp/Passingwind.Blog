using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.UserProfile.Models;
using Passingwind.Blog.Widgets;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.UserProfile
{
	public class UserProfileWidgetView : WidgetComponent
	{
		private readonly BlogUserManager _userManager;

		public UserProfileWidgetView(BlogUserManager userManager)
		{
			_userManager = userManager;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			if (User.Identity?.IsAuthenticated == false)
			{
				return Content("");
			}

			var user = await _userManager.GetUserAsync(User as ClaimsPrincipal);

			if (user == null)
			{
				return Content("");
			}

			var model = new WidgetViewModel()
			{
				Title = WidgetViewContext.ConfigurationInfo.Title,

				Bio = user.Bio,
				DisplayName = user.DisplayName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				UserDescription = user.UserDescription,
				UserName = user.UserName,
			};

			return View(model);
		}
	}
}
