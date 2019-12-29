using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Passingwind.Blog.Web.Controllers
{
	public class TestController : Controller
	{
		private readonly ApplicationPartManager _partManager;

		public TestController(ApplicationPartManager partManager)
		{
			_partManager = partManager;
		}

		public IActionResult Index()
		{

			var controllerFeature = new ControllerFeature();
			_partManager.PopulateFeature(controllerFeature);
			var controllers = controllerFeature.Controllers.ToList();

			var tagHelperFeature = new TagHelperFeature();
			_partManager.PopulateFeature(tagHelperFeature);
			var tagHelpers = tagHelperFeature.TagHelpers.ToList();

			var viewComponentFeature = new ViewComponentFeature();
			_partManager.PopulateFeature(viewComponentFeature);
			var viewComponents = viewComponentFeature.ViewComponents.ToList();

			// RazorCompiledItemFeature


			return Json(new { controllers, tagHelpers, viewComponentFeature });
		}
	}
}