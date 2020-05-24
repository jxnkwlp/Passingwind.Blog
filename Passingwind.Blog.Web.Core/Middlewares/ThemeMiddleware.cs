using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Web.Themes;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Middlewares
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class ThemeMiddleware
	{
		private readonly RequestDelegate _next;

		public ThemeMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext, IThemeAccessor themeAccessor)
		{
			var theme = await themeAccessor.GetCurrentThemeNameAsync();

			httpContext.Items[ThemeConsts.ThemeKey] = theme;

			await _next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ThemeMiddlewareExtensions
	{
		public static IApplicationBuilder UseThemeMiddleware(this IApplicationBuilder builder)
		{
			var app = builder.UseMiddleware<ThemeMiddleware>();
			builder.ApplicationServices.GetRequiredService<IThemeFactory>().Initialize(builder);

			return app;
		}
	}
}
