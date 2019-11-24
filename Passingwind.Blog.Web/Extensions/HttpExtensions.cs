using Microsoft.AspNetCore.Http;

namespace Passingwind.Blog.Web
{
	public static class HttpExtensions
	{
		public static string GetClientIpAddress(this IHttpContextAccessor contextAccessor)
		{
			return contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
		}
	}
}
