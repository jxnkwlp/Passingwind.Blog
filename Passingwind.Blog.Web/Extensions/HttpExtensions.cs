using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Models;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Passingwind.Blog.Web
#pragma warning restore ET002 // Namespace does not match file path or default namespace
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

		public static bool IsCategoryUrl(this HttpRequest request)
		{
			return request.Path.Value.StartsWith("/category/", StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool IsTagsUrl(this HttpRequest request)
		{
			return request.Path.Value.StartsWith("/tag/", StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool IsAuthorUrl(this HttpRequest request)
		{
			return request.Path.Value.StartsWith("/author/", StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool IsDetailsUrl(this HttpRequest request)
		{
			return request.Path.Value.StartsWith("/post/", StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool IsPageUrl(this HttpRequest request)
		{
			return request.Path.Value.StartsWith("/page/", StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool IsSearchUrl(this HttpRequest request)
		{
			return request.Path.Value.StartsWith("/search", StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool IsMonthListUrl(this HttpRequest request)
		{
			Regex reg = new Regex(@"^(\-|\/|\.)[1-2][0-9][0-9][0-9](\-|\/|\.)[0-1][0-9](\-|\/|\.)?$");

			return reg.IsMatch(request.Path.Value);
		}


		public static string PostUrl(this IUrlHelper url, PostViewModel post)
		{
			if (string.IsNullOrEmpty(post.Slug))
			{
				return url.RouteUrl(RouteNames.Post, new { id = post.Id });
			}
			else
			{
				return url.RouteUrl(RouteNames.Post, new { slug = post.Slug });
			}
		}

		public static string GetClientIpAddress(this IHttpContextAccessor contextAccessor)
		{
			return contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
		}
	}
}
