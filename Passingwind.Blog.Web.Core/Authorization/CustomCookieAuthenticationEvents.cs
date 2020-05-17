using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Authorization
{
	public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
	{
		public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
		{
			if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
			{
				// chanages the response status code 
				if (!context.HttpContext.User.Identity.IsAuthenticated)
				{
					context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				}
			}
			else
			{
				base.RedirectToLogin(context);
			}

			return Task.CompletedTask;
		}

		public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
		{
			if (context.Request.Path.StartsWithSegments("/api") )
			{ 
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			}
			else
			{
				base.RedirectToAccessDenied(context);
			}

			return Task.CompletedTask;
		}
		 
	}
}
