using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Models.Account;
using Passingwind.Blog.Web.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ViewComponents
{
	public class ExternalLoginsViewComponent : ViewComponent
	{
		private readonly BlogSignInManager _signInManager;

		public ExternalLoginsViewComponent(BlogSignInManager signInManager)
		{
			_signInManager = signInManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var logins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			return View(new ExternalLoginViewModel() { ExternalLogins = logins });
		}
	}
}
