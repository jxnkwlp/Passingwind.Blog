using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.WebApi;
using System.Net.Mime;

namespace Passingwind.Blog.Web.ApiControllers
{
	[ServiceFilter(typeof(HttpApiResponseExceptionFilter))]
	[ServiceFilter(typeof(HttpApiPermissionActionFilter))]
	[Authorize]
	[ApiController]
	[Produces(MediaTypeNames.Application.Json)]
	[Route("/api/[controller]")]
	public abstract class ApiControllerBase
	{
		#region MyRegion

		//protected IActionResult Ok() => new OkResult();
		//protected IActionResult Ok(object value) => new OkObjectResult(value);

		#endregion

	}
}
