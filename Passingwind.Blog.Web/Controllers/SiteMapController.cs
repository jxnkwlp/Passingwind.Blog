using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Passingwind.Blog.Services;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Passingwind.Blog.Web.Controllers
{
	public class SiteMapController : BlogControllerBase
	{
		private readonly ILogger<SiteMapController> _logger;
		private readonly IPostService _postService;
		private readonly IPageService _pageService;

		public SiteMapController(ILogger<SiteMapController> logger, IPostService postService, IPageService pageService)
		{
			_logger = logger;
			_postService = postService;
			_pageService = pageService;
		}

		[ResponseCache(Duration = 3600)]
		[Route("/sitemap", Name = RouteNames.SiteMap)]
		[HttpGet]
		public async Task<IActionResult> IndexAsync()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				await WriteXmlAsync(ms);

				string result = Encoding.UTF8.GetString(ms.ToArray());

				return Content(result, "text/xml");
			}
		}

		private async Task WriteXmlAsync(Stream stream)
		{
			using (var writer = XmlWriter.Create(stream))
			{
				writer.WriteStartDocument();

				writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

				// Posts
				var posts = (await _postService.GetListAsync(t => !t.IsDraft)).OrderByDescending(t => t.PublishedTime);

				foreach (var post in posts)
				{
					writer.WriteStartElement("url");
					writer.WriteElementString("loc", Url.RouteUrl(RouteNames.Post, new { slug = post.Slug }, Request.Scheme));
					writer.WriteElementString("lastmod", post.PublishedTime.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo));
					writer.WriteElementString("changefreq", "weekly");// always,hourly,daily,weekly,monthly,yearly
					writer.WriteEndElement();
				}

				// Pages
				var pages = await _pageService.GetListAsync(t => t.Published);
				foreach (var page in pages)
				{
					writer.WriteStartElement("url");
					writer.WriteElementString("loc", Url.RouteUrl(RouteNames.Page, new { slug = page.Slug }));
					writer.WriteElementString("lastmod", page.LastModificationTime.HasValue ? page.LastModificationTime.Value.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) : page.CreationTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo));
					writer.WriteElementString("changefreq", "weekly");// always,hourly,daily,weekly,monthly,yearly
					writer.WriteEndElement();
				}

				writer.WriteEndElement();

				writer.WriteEndDocument();
			}
		}
	}
}
