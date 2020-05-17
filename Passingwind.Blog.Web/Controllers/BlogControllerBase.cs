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
		protected void SetPageTitle(string title)
		{
			ViewData["Title"] = title;
		}

		protected void AppendPageTitle(string title)
		{
			if (ViewData["Title"] == null)
				ViewData["Title"] = title;
			else
				ViewData["Title"] = ViewData["Title"] + " | " + title;
		}

		protected void InsertPageTitle(string title)
		{
			if (ViewData["Title"] == null)
				ViewData["Title"] = title;
			else
				ViewData["Title"] = title + " | " + ViewData["Title"];
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
