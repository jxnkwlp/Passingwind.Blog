using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Models;
using System.Diagnostics;

namespace Passingwind.Blog.Web.Controllers
{
	public class HomeController : BlogControllerBase
	{
		[Route("/Error")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
