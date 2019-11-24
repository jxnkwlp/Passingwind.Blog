using Passingwind.Blog.Web;
using Passingwind.Blog.Web.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
	public static class UrlHelperExtensions
	{
		public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
		{
			return urlHelper.Action(
				action: nameof(AccountController.ConfirmEmail),
				controller: "Account",
				values: new { userId, code },
				protocol: scheme);
		}

		public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
		{
			return urlHelper.Action(
				action: nameof(AccountController.ResetPassword),
				controller: "Account",
				values: new { userId, code },
				protocol: scheme);
		}

		public static string PostCommentLink(this IUrlHelper urlHelper, string slug, string scheme, string host, string fragment)
		{
			return urlHelper.RouteUrl(
				routeName: RouteNames.Post,
				values: new { slug },
				protocol: scheme,
				host: host,
				fragment: fragment);
		}

		public static string CaptchaImageLink(this IUrlHelper urlHelper, string id)
		{
			return urlHelper.Action("index", "Captcha", new { id = id });
		}

	}
}
