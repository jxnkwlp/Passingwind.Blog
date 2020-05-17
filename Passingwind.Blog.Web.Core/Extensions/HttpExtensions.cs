using Microsoft.AspNetCore.Http;

namespace Passingwind.Blog.Web.Extensions
{
	public static class HttpExtensions
	{
		public static bool IsPJax(this HttpRequest request)
		{
			return request.Headers.ContainsKey("X-PJAX") && request.Headers["X-PJAX"] == "true";
		}

		public static bool IsAjax(this HttpRequest request)
		{
			return request.Headers.ContainsKey("X-Requested-With") && request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public static string GetClientIpAddress(this IHttpContextAccessor contextAccessor)
		{
			return contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
		}
	}
}
