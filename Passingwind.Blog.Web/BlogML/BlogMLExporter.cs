using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Passingwind.Blog.BlogML
{
    public class BlogMLExporter
    {
        private readonly PostManager _postManager;
        private readonly PageManager _pageManager;
        private readonly CategoryManager _categoryManager;
        private readonly TagsManager _tagsManager;
        private readonly UserManager _userManager;
        private readonly CommentManager _commentManager;

        private readonly ILogger _logger;

        private readonly BasicSettings _basicSettings;

        public BlogMLExporter(ILoggerFactory loggerFactory, PostManager postManager, PageManager pageManager, TagsManager tagsManager, CategoryManager categoryManager, UserManager userManager, CommentManager commentManager, BasicSettings basicSettings)
        {
            this._postManager = postManager;
            this._pageManager = pageManager;
            this._categoryManager = categoryManager;
            this._tagsManager = tagsManager;
            this._commentManager = commentManager;
            this._userManager = userManager;

            this._basicSettings = basicSettings;

            this._logger = loggerFactory.CreateLogger<BlogMLExporter>();
        }

        public string GetExportXml()
        {
            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    writer.WriteStartElement("blog", "http://www.blogml.com/2006/09/BlogML");
                    writer.WriteAttributeString("root-url", "/");
                    writer.WriteAttributeString(
                        "date-created", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");

                    AddTitle(writer);
                    AddSubTitle(writer);
                    AddAuthors(writer);
                    AddExtendedProperties(writer);
                    AddCategories(writer);
                    AddPosts(writer);
                    AddPages(writer);

                    writer.WriteEndElement();
                }

                string result = Encoding.UTF8.GetString(ms.ToArray());

                return result;
            }
        }

        #region Methods

        /// <summary>
        /// Add authors.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void AddAuthors(XmlWriter writer)
        {
            writer.WriteStartElement("authors");

            var allUser = _userManager.GetQueryable().ToList();

            foreach (var user in allUser)
            {
                writer.WriteStartElement("author");

                writer.WriteAttributeString("id", user.UserName);
                writer.WriteAttributeString("date-created", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("date-modified", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("approved", user.IsLockouted.ToString().ToLowerInvariant());
                writer.WriteAttributeString("email", user.Email);

                writer.WriteStartElement("title");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData(user.GetDisplayName());
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Add categories.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void AddCategories(XmlWriter writer)
        {
            var allCategory = _categoryManager.GetQueryable().ToList();

            writer.WriteStartElement("categories");

            foreach (var category in allCategory)
            {
                writer.WriteStartElement("category");

                writer.WriteAttributeString("id", category.Id);
                writer.WriteAttributeString("date-created", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("date-modified", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("approved", "true");
                writer.WriteAttributeString("parentref", category.ParentId);

                if (!String.IsNullOrEmpty(category.Description))
                {
                    writer.WriteAttributeString("description", category.Description);
                }

                writer.WriteStartElement("title");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData(category.Name);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Add extended properties.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void AddExtendedProperties(XmlWriter writer)
        {
            writer.WriteStartElement("extended-properties");

            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", "CommentModeration");
            writer.WriteAttributeString("value", "Anonymous");
            writer.WriteEndElement();

            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", "SendTrackback");
            writer.WriteAttributeString("value", "No");
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Add post author.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="post">
        /// The post to add the author on.
        /// </param>
        private void AddPostAuthor(XmlWriter writer, Post post)
        {
            writer.WriteStartElement("authors");
            writer.WriteStartElement("author");
            writer.WriteAttributeString("ref", post.User.UserName);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Add post categories.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="post">
        /// The post to add categories to.
        /// </param>
        private void AddPostCategories(XmlWriter writer, Post post)
        {
            if (post.Categories.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("categories");
            foreach (var category in post.Categories)
            {
                writer.WriteStartElement("category");
                writer.WriteAttributeString("ref", category.CategoryId);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Add post comments.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="post">
        /// The post to add comments to.
        /// </param>
        private void AddPostComments(XmlWriter writer, Post post)
        {
            var comments = _commentManager.GetQueryable().Where(t => t.PostId == post.Id).OrderByDescending(t => t.CreationTime).ToList();

            writer.WriteStartElement("comments");
            foreach (var comment in comments)
            {
                writer.WriteStartElement("comment");
                writer.WriteAttributeString("id", comment.Id);
                writer.WriteAttributeString("parentid", comment.ParentId);
                writer.WriteAttributeString("date-created", comment.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("date-modified", comment.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("approved", comment.IsApproved.ToString().ToLowerInvariant());
                writer.WriteAttributeString("user-name", comment.Author);
                writer.WriteAttributeString("user-email", comment.Email);
                writer.WriteAttributeString("user-ip", comment.IP);

                if (comment.Website != null)
                {
                    writer.WriteAttributeString("user-url", comment.Website);
                }
                else
                {
                    writer.WriteAttributeString("user-url", string.Empty);
                }

                writer.WriteStartElement("title");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData("re: " + post.Title);
                writer.WriteEndElement();

                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData(comment.Content);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Add post content.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="post">
        /// The post to add content to.
        /// </param>
        private void AddPostContent(XmlWriter writer, Post post)
        {
            writer.WriteStartElement("content");
            writer.WriteAttributeString("type", "text");
            writer.WriteCData(post.Content);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the post excerpt.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="post">The post to add the excerpt to.</param>
        private void AddPostExcerpt(XmlWriter writer, Post post)
        {
            if (String.IsNullOrEmpty(post.Description))
            {
                return;
            }

            writer.WriteStartElement("excerpt");
            writer.WriteAttributeString("type", "text");
            writer.WriteCData(post.Description);
            writer.WriteEndElement();
        }

        /// <summary>
        /// The post-name element contains the same as the title.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="post">
        /// The post to add a name to.
        /// </param>
        private void AddPostName(XmlWriter writer, Post post)
        {
            writer.WriteStartElement("post-name");
            writer.WriteAttributeString("type", "text");
            writer.WriteCData(post.Title);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the post tags.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="post">The post to add tags to.</param>
        private void AddPostTags(XmlWriter writer, Post post)
        {
            if (post.Tags.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("tags");
            foreach (var tag in post.Tags)
            {
                writer.WriteStartElement("tag");
                writer.WriteAttributeString("ref", tag.Tags.Name);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the post title.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="post">The post to add the title to.</param>
        private void AddPostTitle(XmlWriter writer, Post post)
        {
            writer.WriteStartElement("title");
            writer.WriteAttributeString("type", "text");
            writer.WriteCData(post.Title);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the post trackbacks.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="post">The post to add trackbacks for.</param>
        private void AddPostTrackbacks(XmlWriter writer, Post post)
        {
            if (post.Comments.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("trackbacks");
            foreach (var comment in
                post.Comments.Where(comment => comment.Email == "trackback" || comment.Email == "pingback"))
            {
                writer.WriteStartElement("trackback");
                writer.WriteAttributeString("id", comment.Id);
                writer.WriteAttributeString("date-created", comment.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("date-modified", comment.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("approved", comment.IsApproved.ToString().ToLowerInvariant());

                if (comment.Website != null)
                {
                    writer.WriteAttributeString("url", comment.Website);
                }

                writer.WriteStartElement("title");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData(comment.Content);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the posts.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void AddPosts(XmlWriter writer)
        {
            var allPosts = _postManager.GetQueryable().ToList();

            writer.WriteStartElement("posts");

            foreach (var post in allPosts)
            {
                writer.WriteStartElement("post");

                writer.WriteAttributeString("id", post.Id);
                writer.WriteAttributeString("date-created", post.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                if (post.LastModificationTime.HasValue)
                    writer.WriteAttributeString("date-modified", post.LastModificationTime.Value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("approved", "true");
                writer.WriteAttributeString("post-url", "/post/" + post.Slug);
                writer.WriteAttributeString("type", "normal");
                writer.WriteAttributeString("hasexcerpt", (!string.IsNullOrEmpty(post.Description)).ToString().ToLowerInvariant());
                writer.WriteAttributeString("views", post.ViewsCount.ToString());
                writer.WriteAttributeString("is-published", (!post.IsDraft).ToString().ToLowerInvariant());

                AddPostTitle(writer, post);
                AddPostContent(writer, post);
                AddPostName(writer, post);
                AddPostExcerpt(writer, post);
                AddPostAuthor(writer, post);
                AddPostCategories(writer, post);
                AddPostTags(writer, post);
                AddPostComments(writer, post);
                AddPostTrackbacks(writer, post);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void AddPages(XmlWriter writer)
        {
            var allPages = _pageManager.GetQueryable().ToList();

            writer.WriteStartElement("posts");

            foreach (var post in allPages)
            {
                writer.WriteStartElement("post");

                writer.WriteAttributeString("id", post.Id);
                writer.WriteAttributeString(
                    "date-created", post.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                if (post.LastModificationTime.HasValue)
                    writer.WriteAttributeString("date-modified", post.LastModificationTime.Value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("approved", "true");
                writer.WriteAttributeString("post-url", "/page/" + post.Slug);
                writer.WriteAttributeString("type", "article");  // "normal" for posts and "article" for pages
                writer.WriteAttributeString(
                    "hasexcerpt", (!string.IsNullOrEmpty(post.Description)).ToString().ToLowerInvariant());
                writer.WriteAttributeString("views", "0");
                writer.WriteAttributeString("is-published", post.Published.ToString().ToLowerInvariant());

                writer.WriteStartElement("title");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData(post.Title);
                writer.WriteEndElement();

                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData(post.Content);
                writer.WriteEndElement();

                writer.WriteStartElement("post-name");
                writer.WriteAttributeString("type", "text");
                writer.WriteCData(post.Title);
                writer.WriteEndElement();

                writer.WriteStartElement("authors");
                //writer.WriteStartElement("author");
                //writer.WriteAttributeString("ref", HttpContext.Current.User.Identity.Name);
                //writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the sub title.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void AddSubTitle(XmlWriter writer)
        {
            writer.WriteStartElement("sub-title");
            writer.WriteAttributeString("type", "text");
            writer.WriteCData(_basicSettings.Description);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the title.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void AddTitle(XmlWriter writer)
        {
            writer.WriteStartElement("title");
            writer.WriteAttributeString("type", "text");
            writer.WriteCData(_basicSettings.Title);
            writer.WriteEndElement();
        }

        ///// <summary>
        ///// Writes the BlogML to the output stream.
        ///// </summary>
        ///// <param name="context">
        ///// The context.
        ///// </param>
        //private void WriteXml(HttpContext context)
        //{
        //    var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

        //    using (var writer = XmlWriter.Create(context.Response.OutputStream, settings))
        //    {
        //        writer.WriteStartElement("blog", "http://www.blogml.com/2006/09/BlogML");
        //        writer.WriteAttributeString("root-url", Utils.RelativeWebRoot);
        //        writer.WriteAttributeString(
        //            "date-created", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
        //        writer.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");

        //        AddTitle(writer);
        //        AddSubTitle(writer);
        //        AddAuthors(writer);
        //        AddExtendedProperties(writer);
        //        AddCategories(writer);
        //        AddPosts(writer);
        //        AddPages(writer);

        //        writer.WriteEndElement();
        //    }
        //}

        #endregion
    }
}
