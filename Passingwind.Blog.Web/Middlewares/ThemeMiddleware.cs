using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Passingwind.Blog.Web
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class ThemeMiddleware
	{
		private const string ThemeKey = "blog.theme";

		private readonly RequestDelegate _next;

		public ThemeMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public Task Invoke(HttpContext httpContext)
		{
			//httpContext.Items[ThemeKey] = "Abc";

			return _next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ThemeMiddlewareExtensions
	{
		public static IApplicationBuilder UseThemeMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ThemeMiddleware>();
		}
	}
}
