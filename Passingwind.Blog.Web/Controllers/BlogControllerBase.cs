using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Passingwind.Blog.Web.Controllers
{
	/// <summary>
	/// The basic 
	/// </summary>
	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("[controller]/[action]")]
	public abstract class BlogControllerBase : Controller
	{ 
		protected void AppendPageTitle(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;

			if (ViewData["Title"] == null)
				ViewData["Title"] = value;
			else
				ViewData["Title"] = ViewData["Title"] + " - " + value;
		}

		protected void InsertPageTitle(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;

			if (ViewData["Title"] == null)
				ViewData["Title"] = value;
			else
				ViewData["Title"] = value + " - " + ViewData["Title"];
		}

		protected void SetPageDescription(string text)
		{
			ViewData["Description"] = text;
		}

		protected void SetPageKeywords(string text)
		{
			ViewData["Keywords"] = text;
		}

	}
}
