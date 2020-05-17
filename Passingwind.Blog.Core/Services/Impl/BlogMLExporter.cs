using Microsoft.Extensions.Logging;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Settings;
using Passingwind.Blog.Services.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Passingwind.Blog.Services.Impl
{
	public class BlogMLExporter : IBlogMLExporter
	{
		private BlogMLExportOptions _blogMLExportOptions;
		private User _user;

		private readonly ILogger<BlogMLExporter> _logger;
		private readonly IPostService _postService;
		private readonly IPageService _pageService;
		private readonly ICategoryService _categoryService;
		private readonly ITagsService _tagsService;
		private readonly ICommentService _commentService;
		private readonly ISlugService _slugService;
		private readonly BlogUserManager _userManager;
		private readonly BasicSettings _basicSettings;

		public BlogMLExporter(ILogger<BlogMLExporter> logger, IPostService postService, IPageService pageService, ICategoryService categoryService, ITagsService tagsService, ICommentService commentService, ISlugService slugService, BlogUserManager userManager, BasicSettings basicSettings)
		{
			_logger = logger;
			_postService = postService;
			_pageService = pageService;
			_categoryService = categoryService;
			_tagsService = tagsService;
			_commentService = commentService;
			_slugService = slugService;
			_userManager = userManager;
			_basicSettings = basicSettings;
		}

		public async Task<byte[]> ExportAsync(User user, BlogMLExportOptions options)
		{
			_user = user;
			_blogMLExportOptions = options;

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
					await AddAuthorsAsync(writer);
					AddExtendedProperties(writer);
					await AddCategoriesAsync(writer);
					await AddPostsAsync(writer);
					await AddPagesAsync(writer);

					writer.WriteEndElement();
				}

				return ms.ToArray();
			}
		}


		/// <summary>
		/// Add authors.
		/// </summary>
		/// <param name="writer">
		/// The writer.
		/// </param>
		private async Task AddAuthorsAsync(XmlWriter writer)
		{
			writer.WriteStartElement("authors");

			if (_blogMLExportOptions.ExportAllUser)
			{
				var allUser = await _userManager.GetUserListAsync(new UserPagedListInputModel());

				foreach (var user in allUser)
				{
					writer.WriteStartElement("author");

					writer.WriteAttributeString("id", user.UserName);
					writer.WriteAttributeString("date-created", user.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
					writer.WriteAttributeString("date-modified", user.LastModificationTime.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
					writer.WriteAttributeString("approved", user.IsLockouted.ToString().ToLowerInvariant());
					writer.WriteAttributeString("email", user.Email);

					writer.WriteStartElement("title");
					writer.WriteAttributeString("type", "text");
					writer.WriteCData(user.GetDisplayName());
					writer.WriteEndElement();

					writer.WriteEndElement();
				}
			}
			else
			{
				var user = _user;

				writer.WriteStartElement("author");

				writer.WriteAttributeString("id", user.UserName);
				writer.WriteAttributeString("date-created", user.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				writer.WriteAttributeString("date-modified", user.LastModificationTime.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
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
		private async Task AddCategoriesAsync(XmlWriter writer)
		{
			var allCategory = await _categoryService.GetListAsync();

			writer.WriteStartElement("categories");

			foreach (var category in allCategory)
			{
				writer.WriteStartElement("category");

				writer.WriteAttributeString("id", category.Id.ToString());
				writer.WriteAttributeString("date-created", category.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				writer.WriteAttributeString("date-modified", category.LastModificationTime.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				writer.WriteAttributeString("approved", "true");
				writer.WriteAttributeString("parentref", category.ParentId.ToString());

				if (!string.IsNullOrEmpty(category.Description))
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
				writer.WriteAttributeString("ref", category.CategoryId.ToString());
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
		private async Task AddPostCommentsAsync(XmlWriter writer, Post post)
		{
			var comments = await _commentService.GetCommentsByPostId(post.Id, true);

			writer.WriteStartElement("comments");
			foreach (var comment in comments)
			{
				writer.WriteStartElement("comment");
				writer.WriteAttributeString("id", comment.Id.ToString());
				writer.WriteAttributeString("parentid", comment.ParentId.ToString());
				writer.WriteAttributeString("date-created", comment.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				writer.WriteAttributeString("date-modified", comment.LastModificationTime.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				writer.WriteAttributeString("approved", comment.IsApproved.ToString().ToLowerInvariant());
				writer.WriteAttributeString("user-name", comment.Author);
				writer.WriteAttributeString("user-email", comment.Email);
				writer.WriteAttributeString("user-ip", comment.IP);

				writer.WriteAttributeString("user-url", comment.Website ?? string.Empty);

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
			if (string.IsNullOrEmpty(post.Description))
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
				writer.WriteAttributeString("id", comment.Id.ToString());
				writer.WriteAttributeString("date-created", comment.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				writer.WriteAttributeString("date-modified", comment.LastModificationTime.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
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
		private async Task AddPostsAsync(XmlWriter writer)
		{
			var allPosts = await _postService.GetPostListAsync(new PostListInputModel()
			{
				IncludeOptions = new PostIncludeOptions()
				{
					IncludeCategory = true,
					IncludeTags = true,
					IncludeUser = true,
				}
			});

			writer.WriteStartElement("posts");

			foreach (var post in allPosts)
			{
				writer.WriteStartElement("post");

				writer.WriteAttributeString("id", post.Id.ToString());
				writer.WriteAttributeString("date-created", post.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				if (post.LastModificationTime.HasValue)
					writer.WriteAttributeString("date-modified", post.LastModificationTime.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
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
				await AddPostCommentsAsync(writer, post);
				AddPostTrackbacks(writer, post);

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		private async Task AddPagesAsync(XmlWriter writer)
		{
			var allPages = await _pageService.GetListAsync();

			writer.WriteStartElement("posts");

			foreach (var post in allPages)
			{
				writer.WriteStartElement("post");

				writer.WriteAttributeString("id", post.Id.ToString());
				writer.WriteAttributeString(
					"date-created", post.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
				if (post.LastModificationTime.HasValue)
					writer.WriteAttributeString("date-modified", post.LastModificationTime.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
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
	}
}
