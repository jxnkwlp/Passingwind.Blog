using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Text;
using System.Globalization;
using System.IO;

namespace Passingwind.Blog.Web.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly PostManager _postManager;
        private readonly PageManager _pageManager;

        public SiteMapController(ILoggerFactory loggerFactory, PostManager postManager, PageManager pageManager)
        {
            this._postManager = postManager;
            this._pageManager = pageManager;
        }

        [ResponseCache(Duration = 3600)]
        [Route("sitemap.xml", Name = RouteNames.SiteMap)]
        public IActionResult Index()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteXml(ms);

                string result = Encoding.UTF8.GetString(ms.ToArray());

                return Content(result, "text/xml");
            }
        }


        private void WriteXml(Stream stream)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                // Posts
                var posts = _postManager.GetQueryable().Where(t => !t.IsDraft).ToList();
                foreach (var post in posts)
                {
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", Url.RouteUrl(RouteNames.Post, new { slug = post.Slug }, Request.Scheme));
                    writer.WriteElementString("lastmod", post.PublishedTime.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo));
                    writer.WriteElementString("changefreq", "weekly");// always,hourly,daily,weekly,monthly,yearly
                    writer.WriteEndElement();
                }

                // Pages
                var pages = _pageManager.GetQueryable().Where(t => t.Published).ToList();
                foreach (var page in pages)
                {
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", Url.RouteUrl(RouteNames.Page, new { slug = page.Slug }, Request.Scheme));
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
