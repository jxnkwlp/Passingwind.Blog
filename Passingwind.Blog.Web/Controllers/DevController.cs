using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Web.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class DevController : Controller
	{
		private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

		public DevController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
		{
			_actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
		}

		//public IActionResult Index()
		//{
		//	return View();
		//}

		[HttpPost]
		public IActionResult SetLanguage(string culture, string returnUrl)
		{
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			return LocalRedirect(returnUrl);
		}

		[HttpGet("/_routes")]
		public IActionResult GetAllRoutes()
		{
			var all = _actionDescriptorCollectionProvider.ActionDescriptors.Items;

			var result = new List<RouteFormatInfomation>();

			foreach (var item in all)
			{
				RouteFormatInfomation info = new RouteFormatInfomation();

				if (item.RouteValues.TryGetValue("area", out var area))
					info.Area = area;

				if (item.RouteValues.TryGetValue("controller", out var controller))
					info.Controller = controller;

				if (item.RouteValues.TryGetValue("action", out var action))
					info.Action = action;

				if (item.RouteValues.TryGetValue("page", out var page))
					info.Page = page;


				if (item is PageActionDescriptor pageActionDescriptor)
				{
					//pageActionDescriptor.ViewEnginePath = pageActionDescriptor.ViewEnginePath.Replace("/Pages", "");
					info.ViewEnginePath = pageActionDescriptor.ViewEnginePath;
					info.ViewRelativePath = pageActionDescriptor.RelativePath;
					info.DisplayName = pageActionDescriptor.DisplayName;

				}

				info.RouteInfo = item.AttributeRouteInfo;

				if (item is ControllerActionDescriptor controllerActionDescriptor)
				{
				}


				result.Add(info);
			}

			return Ok(result);
		}
	}

	public class RouteFormatInfomation
	{
		public string Area { get; set; }
		public string Controller { get; set; }
		public string Action { get; set; }
		public string Page { get; set; }

		public string ViewEnginePath { get; set; }
		public string ViewRelativePath { get; set; }
		public string DisplayName { get; set; }

		public AttributeRouteInfo RouteInfo { get; set; }
	}
}
