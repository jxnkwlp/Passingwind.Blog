using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web
{
	/// <summary>
	///  /image.axd?picture=/filename.png ==> uplods/picture/filename.png
	/// </summary>
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class ImageAxdMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly UrlEncoder _urlEncoder;

		public ImageAxdMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment, UrlEncoder urlEncoder)
		{
			_next = next;
			_hostingEnvironment = hostingEnvironment;
			_urlEncoder = urlEncoder;
		}

		public Task Invoke(HttpContext httpContext)
		{
			if (httpContext == null)
				throw new System.ArgumentNullException(nameof(httpContext));

			if (httpContext.Request.Path.Value.StartsWith("/image.axd"))
			{
				StringValues file = new StringValues();
				if (httpContext.Request.Query.TryGetValue("picture", out file))
				{
					var pathFragment = file.ToString().Split('/');

					var newPath = string.Join("/", pathFragment.Select(t => System.Net.WebUtility.UrlEncode(t)));

					var filePath = Path.Combine($"/uploads/picture{newPath}");

					httpContext.Response.Redirect(filePath, false);
				}

				return Task.FromResult(0);
			}
			else
				return _next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ImageAxdMiddlewareExtensions
	{
		public static IApplicationBuilder UseImageAxdMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ImageAxdMiddleware>();
		}
	}
}
