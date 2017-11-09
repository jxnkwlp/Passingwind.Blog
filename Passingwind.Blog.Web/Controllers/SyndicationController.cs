using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Passingwind.Blog.Web.Controllers
{
    public class SyndicationController : Controller
    {
        const int DefaultMaxSouceCount = 100;

        private readonly PageManager _pageManager;
        private readonly PostManager _postManager;
        private readonly CategoryManager _categoryManager;

        private readonly BasicSettings _basicSettings;
        private readonly AdvancedSettings _advancedSettings;
        private readonly FeedSettings _feedSettings;

        public SyndicationController(PageManager pageManager, PostManager postManager, CategoryManager categoryManager, BasicSettings basicSettings, AdvancedSettings advancedSettings, FeedSettings feedSettings)
        {
            this._postManager = postManager;
            this._pageManager = pageManager;
            this._categoryManager = categoryManager;

            this._basicSettings = basicSettings;
            this._advancedSettings = advancedSettings;
            this._feedSettings = feedSettings;

        }


        [Route("syndication.xml", Name = RouteNames.Syndication)]
        //[ResponseCache(Duration = 3600 * 12)]
        public IActionResult Index(string format = "rss")
        {
            var posts = GetPosts(null);

            return SyndicationResult(posts, format);
        }

        [Route("category/{slug}/syndication.xml", Name = RouteNames.SyndicationCategory)]
        //[OutputCache(Duration = 3600 * 12)]
        public async Task<ActionResult> Category(string slug, string format = "rss")
        {
            if (string.IsNullOrEmpty(slug))
                return View("NotFound");

            var category = await _categoryManager.GetBySlugAsync(slug);

            if (category == null)
                return View("NotFound");

            var posts = GetPosts(category);

            return SyndicationResult(posts, format);
        }

        protected ActionResult SyndicationResult(IList<Post> posts, string format = "rss")
        {
            SyndicationFormat sf = SyndicationFormat.Rss;

            if (!Enum.TryParse<SyndicationFormat>(format, true, out sf))
            {
                return View("NotFound");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                if (sf == SyndicationFormat.Rss)
                {
                    WriteRssXml(ms, posts);
                }
                else if (sf == SyndicationFormat.Atom)
                {
                    WriteAtomXml(ms, posts);
                }
                else
                {
                    return View("NotFound");
                }

                string result = Encoding.UTF8.GetString(ms.ToArray());

                return Content(result, "text/xml");
            }
        }

        protected IDictionary<string, string> SupportedNamespaces
        {
            get
            {
                return new Dictionary<string, string>
                            {
                                { "blogChannel", "http://backend.userland.com/blogChannelModule" },
                                { "dc", "http://purl.org/dc/elements/1.1/" },
                                { "pingback", "http://madskills.com/public/xml/rss/module/pingback/" },
                                { "trackback", "http://madskills.com/public/xml/rss/module/trackback/" },
                                { "wfw", "http://wellformedweb.org/CommentAPI/" },
                                { "slash", "http://purl.org/rss/1.0/modules/slash/" },
                                { "geo", "http://www.w3.org/2003/01/geo/wgs84_pos#" },
                            };
            }
        }

        private IList<Post> GetPosts(Category category = null)
        {
            int max = _feedSettings.ShowCount > 0 ? _feedSettings.ShowCount : DefaultMaxSouceCount;

            var query = _postManager.GetQueryable().Where(t => !t.IsDraft);

            if (category != null)
            {
                query = query.Where(t => t.Categories.Any(c => c.CategoryId == category.Id));
            }

            var posts = query.Take(max).ToList();

            return posts;
        }

        #region Rss

        private void WriteRssItems(XmlWriter writer, IList<Post> posts)
        {
            foreach (var post in posts)
            {
                writer.WriteStartElement("item");

                // required element
                writer.WriteElementString("title", post.Title);
                writer.WriteElementString("description", string.IsNullOrEmpty(post.Description) ? post.Content : post.Description);
                string postUrl = Url.RouteUrl(RouteNames.Post, new { slug = post.Slug }, Request.Scheme);
                writer.WriteElementString("link", postUrl);

                // optionals element
                if (post.User != null)
                    writer.WriteElementString("author", post.User.GetDisplayName());

                writer.WriteElementString("guid", postUrl);
                writer.WriteElementString("pubDate", ConventToRFC822(post.PublishedTime));

                if (post.Categories != null)
                    foreach (var category in post.Categories)
                    {
                        if (category.Category != null)
                            writer.WriteElementString("category", category.Category.Name);
                    }


                writer.WriteEndElement();
            }
        }

        private void WriteRssXml(Stream stream, IList<Post> posts)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                writer.WriteStartElement("rss");
                writer.WriteAttributeString("version", "2.0");

                // channel
                writer.WriteStartElement("channel");

                writer.WriteElementString("title", _basicSettings.Title);
                writer.WriteElementString("description", _basicSettings.Description);
                writer.WriteElementString("link", Request.Host.ToUriComponent());

                WriteRssItems(writer, posts);

                writer.WriteEndElement();

                // channel end.

                writer.WriteFullEndElement();
            }
        }

        #endregion

        #region atom

        private void WriteAtomItems(XmlWriter writer, IList<Post> posts)
        {
            foreach (var post in posts)
            {
                string postUrl = Url.RouteUrl(RouteNames.Post, new { slug = post.Slug }, Request.Scheme);

                writer.WriteStartElement("entry");

                // required element
                writer.WriteElementString("id", postUrl);
                writer.WriteElementString("title", post.Title);
                writer.WriteElementString("updated", ConventToW3CDateTime(post.PublishedTime));

                // recommended element
                writer.WriteStartElement("link");
                writer.WriteAttributeString("href", postUrl);
                writer.WriteEndElement();

                if (post.User != null)
                {
                    writer.WriteStartElement("author");
                    writer.WriteElementString("name", post.User.GetDisplayName());
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("summary");
                writer.WriteAttributeString("type", "html");
                writer.WriteString(post.Description);
                writer.WriteEndElement();


                // optionals element
                writer.WriteElementString("published", ConventToW3CDateTime(post.PublishedTime));

                if (post.Categories != null)
                    foreach (var category in post.Categories)
                    {
                        if (category.Category != null)
                        {
                            writer.WriteStartElement("category");
                            writer.WriteAttributeString("term", category.Category.Name);
                            writer.WriteEndElement();
                        }
                    }


                writer.WriteEndElement();
            }
        }

        private void WriteAtomXml(Stream stream, IList<Post> posts)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                writer.WriteStartElement("feed", "http://www.w3.org/2005/Atom");

                writer.WriteElementString("id", Request.Host.ToUriComponent());
                writer.WriteElementString("title", _basicSettings.Title);

                var lastUpdated = posts.Max(t => t.PublishedTime);
                writer.WriteElementString("updated", ConventToW3CDateTime(lastUpdated));

                writer.WriteStartElement("link");
                writer.WriteAttributeString("href", Request.Host.ToUriComponent());
                writer.WriteEndElement();

                writer.WriteStartElement("link");
                writer.WriteAttributeString("rel", "self");
                writer.WriteAttributeString("href", Url.RouteUrl(RouteNames.Syndication, new { }, Request.Scheme) + "?format=atom");
                writer.WriteEndElement();

                writer.WriteElementString("subtitle", _basicSettings.Description);

                // optional elements
                //writer.WriteStartElement("author");
                //writer.WriteElementString("name", "");
                //writer.WriteEndElement();

                //writer.WriteStartElement("generator");
                //writer.WriteAttributeString("uri", "");
                //writer.WriteAttributeString("version", "");
                //writer.WriteString("");
                //writer.WriteEndElement();

                WriteAtomItems(writer, posts);


                writer.WriteFullEndElement();
            }
        }


        #endregion

        /// <summary>
        ///  Wed, 04 Mar 2009 09:42:31 GMT
        /// </summary>
        static string ConventToRFC822(DateTime pubDate)
        {
            return pubDate.ToString("r");

            //// get a timezone from local time (won't work with UTC)
            //var zone = DateTime.Now.ToString("zzzz").Replace(":", "");

            //var value = pubDate.ToString("ddd',' d MMM yyyy HH':'mm':'ss") + " " + zone;
            //return value;
        }
        static string ConventToW3CDateTime(DateTime utcDateTime)
        {
            var utcOffset = TimeSpan.Zero;
            return (utcDateTime + utcOffset).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) +
                   FormatW3COffset(utcOffset, ":");
        }

        static string FormatW3COffset(TimeSpan offset, string separator)
        {
            var formattedOffset = String.Empty;

            if (offset >= TimeSpan.Zero)
            {
                formattedOffset = "+";
            }

            return String.Concat(
                formattedOffset,
                offset.Hours.ToString("00", CultureInfo.InvariantCulture),
                separator,
                offset.Minutes.ToString("00", CultureInfo.InvariantCulture));
        }
    }

    public enum SyndicationFormat
    {
        /// <summary>
        ///     No syndication format specified.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Indicates that a feed conforms to the Atom syndication format.
        /// </summary>
        Atom = 1,

        /// <summary>
        ///     Indicates that a feed conforms to the RSS syndication format.
        /// </summary>
        Rss = 2
    }
}
