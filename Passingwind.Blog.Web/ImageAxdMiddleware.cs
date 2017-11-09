using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Primitives;
using System.Text.Encodings.Web;

namespace Passingwind.Blog.Web
{
    /// <summary>
    ///  /image.axd?picture=/filename.png ==> uplods/picture/filename.png
    /// </summary>
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ImageAxdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UrlEncoder _urlEncoder;

        public ImageAxdMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment, UrlEncoder urlEncoder)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _urlEncoder = urlEncoder;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.Value.StartsWith("/image.axd"))
            {
                if (httpContext.Request.Query.TryGetValue("picture", out StringValues pciture))
                {
                    var path = pciture.ToString();
                    // path = path.Remove(0, 8);

                    //var pictureServerDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads/picture");
                    //if (!Directory.Exists(pictureServerDirectory))
                    //{
                    //    Directory.CreateDirectory(pictureServerDirectory);
                    //}

                    var filePath = Path.Combine("Uploads/picture", (path));

                    httpContext.Response.Redirect(_urlEncoder.Encode(filePath), true);

                    return Task.FromResult(0);

                    // var fileServerPath = Path.Combine(_hostingEnvironment.WebRootPath, filePath);

                    // httpContext.Response.WriteAsync()
                }
            }

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
