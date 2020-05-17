using BlogML;
using BlogML.Xml;
using Microsoft.Extensions.Logging;
using Passingwind.Blog.Data.Domains;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Passingwind.Blog.Services.Impl
{
	public class BlogMLImporter : IBlogMLImporter
	{
		private XmlReader XmlReader
		{
			get
			{
				var byteArray = Encoding.UTF8.GetBytes(_xml);
				var stream = new MemoryStream(byteArray);
				return XmlReader.Create(stream);
			}
		}

		private User _user;
		private string _xml;

		private readonly List<Category> categoryLookup = new List<Category>();
		private readonly List<BlogMlExtendedPost> blogsExtended = new List<BlogMlExtendedPost>();
		private readonly List<BlogMlExtendedPost> blogsPages = new List<BlogMlExtendedPost>();
		private Dictionary<string, Dictionary<string, int>> _substitueIds = new Dictionary<string, Dictionary<string, int>>();


		private readonly ILogger<BlogMLImporter> _logger;
		private readonly IPostService _postService;
		private readonly IPageService _pageService;
		private readonly ICategoryService _categoryService;
		private readonly ITagsService _tagsService;
		private readonly ICommentService _commentService;
		private readonly IIPAddressService _iPAddressService;
		private readonly ISlugService _slugService;


		public int PostCount { get; set; }
		public int PageCount { get; set; }
		public int CategoryCount { get; set; }


		public BlogMLImporter(ILogger<BlogMLImporter> logger, IPostService postService, IPageService pageService, ICategoryService categoryService, ITagsService tagsService, ICommentService commentService, IIPAddressService iPAddressService, ISlugService slugService)
		{
			_logger = logger;
			_postService = postService;
			_pageService = pageService;
			_categoryService = categoryService;
			_tagsService = tagsService;
			_commentService = commentService;
			_iPAddressService = iPAddressService;
			_slugService = slugService;
		}

		/// <summary>
		///  setps:
		///  1, import category,tags, authors ...
		///  2, import post, page, command
		/// </summary> 
		public async Task<BlogMLImporterResult> ImportAsync(User user, string xml)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (xml == null)
				throw new ArgumentNullException(nameof(xml));

			_xml = xml;
			_user = user;

			var blog = new BlogMLBlog();

			try
			{
				blog = BlogMLSerializer.Deserialize(XmlReader);
			}
			catch (Exception ex)
			{
				string message = string.Format("BlogML could not load with 2.0 specs. {0}", ex.Message);

				_logger.LogError(message);
			}

			try
			{
				await ImportCategoryAsync(blog);

				LoadFromXmlDocument();

				await ImportPostsAsync(blog);

				_logger.LogInformation("Import completed.");

				return new BlogMLImporterResult()
				{
					CategoryCount = CategoryCount,
					PageCount = PageCount,
					PostCount = PostCount,
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "BlogML import failed.");
				throw;
			}
		}

		#region Utils

		private T GetAttributeValue<T>(XmlAttribute attr)
		{
			if (attr == null)
				return default(T);

			return (T)Convert.ChangeType(attr.Value, typeof(T));
		}

		private T GetAttributeValue<T>(XmlNode node, string attrName)
		{
			var attr = node.Attributes[attrName];

			if (attr == null)
				return default(T);

			return (T)Convert.ChangeType(attr.Value, typeof(T));
		}

		private int GetOrAddId(string type, string oldValue, int? newValue = null)
		{
			if (_substitueIds == null)
			{
				_substitueIds = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
			}

			if (!_substitueIds.ContainsKey(type))
				_substitueIds.Add(type, new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

			if (newValue.HasValue)
			{
				if (!_substitueIds[type].ContainsKey(oldValue))
					_substitueIds[type].Add(oldValue, newValue.Value);

				return _substitueIds[type][oldValue];
			}
			else
			{
				return _substitueIds[type][oldValue];
			}
		}

		private DateTime GetDate(XmlAttribute attr)
		{
			string value = GetAttributeValue<string>(attr);
			DateTime defaultDate = DateTime.Now;

			DateTime dt = defaultDate;
			if (!string.IsNullOrWhiteSpace(value))
			{
				if (!DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
					dt = defaultDate;
			}

			return dt;
		}

		private Uri GetUri(string value)
		{
			if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri uri))
				return uri;

			return null;
		}

		private int GetValue(object value)
		{
			if (value == null)
				return 0;

			if (int.TryParse(value.ToString(), out int i))
				return i;

			return 0;
		}

		#endregion

		private async Task ImportCategoryAsync(BlogMLBlog blogMLBlog)
		{
			_logger.LogInformation("Start importing categoies...");

			foreach (var cat in blogMLBlog.Categories)
			{
				var name = cat.Title;
				var slug = await _slugService.NormalarAsync(name);

				var entity = await _categoryService.GetBySlugAsync(slug);

				if (entity == null)
				{
					entity = new Category
					{
						Name = cat.Title,
						Slug = slug,
						Description = cat.Description,
						DisplayOrder = 1,
						CreationTime = cat.DateCreated,
						LastModificationTime = cat.DateModified,
					};

					if (!string.IsNullOrEmpty(cat.ParentRef) && cat.ParentRef != "0")
						entity.ParentId = GetOrAddId("category", cat.ParentRef);

					await _categoryService.InsertAsync(entity);

					CategoryCount++;
				}

				GetOrAddId("category", cat.ID, entity.Id);
				categoryLookup.Add(entity);
			}
		}


		private void LoadFromXmlDocument()
		{
			var doc = new XmlDocument();

			doc.Load(XmlReader);

			var posts = doc.GetElementsByTagName("post");

			foreach (XmlNode post in posts)
			{
				var blogX = new BlogMlExtendedPost();

				if (post.Attributes != null)
					blogX.PostUrl = GetAttributeValue<string>(post.Attributes["post-url"]);

				if (post.ChildNodes.Count <= 0)
				{
					blogsExtended.Add(blogX);
					continue;
				}

				foreach (XmlNode child in post.ChildNodes)
				{
					if (child.Name == "tags")
					{
						foreach (XmlNode tag in child.ChildNodes)
						{
							if (tag.Attributes != null)
							{
								if (blogX.Tags == null)
									blogX.Tags = new List<string>();

								blogX.Tags.Add(GetAttributeValue<string>(tag.Attributes["ref"]));
							}
						}
					}

					if (child.Name == "comments")
						LoadBlogComments(blogX, child);

					if (child.Name == "trackbacks")
						LoadBlogTrackbacks(blogX, child);
				}

				blogsExtended.Add(blogX);
			}
		}

		private void LoadBlogComments(BlogMlExtendedPost blogX, XmlNode child)
		{
			foreach (XmlNode com in child.ChildNodes)
			{
				if (com.Attributes != null)
				{
					var c = new ExtendedComment
					{
						CommentId = GetAttributeValue<string>(com.Attributes["id"]),
						Author = GetAttributeValue<string>(com.Attributes["user-name"]),
						Email = GetAttributeValue<string>(com.Attributes["user-email"]),
						IP = GetAttributeValue<string>(com.Attributes["user-ip"]),
						CreationTime = GetDate(com.Attributes["date-created"]),
						Content = "",
						Website = GetAttributeValue<string>(com, "user-url"),
					};

					var parentid = GetAttributeValue<string>(com.Attributes["parentid"]);
					if (!string.IsNullOrEmpty(parentid))
						c.ParentCommentId = parentid;

					c.IsApproved = GetAttributeValue<bool>(com.Attributes["approved"]);

					foreach (XmlNode comNode in com.ChildNodes)
					{
						if (comNode.Name == "content")
						{
							c.Content = comNode.InnerText;
						}
					}

					if (blogX.Comments == null)
						blogX.Comments = new List<ExtendedComment>();

					blogX.Comments.Add(c);
				}
			}
		}

		private void LoadBlogTrackbacks(BlogMlExtendedPost blogX, XmlNode child)
		{
			foreach (XmlNode com in child.ChildNodes)
			{
				if (com.Attributes != null)
				{
					var c = new ExtendedComment
					{
						IP = "127.0.0.1",
						IsApproved = GetAttributeValue<bool>(com.Attributes["approved"]),
						CreationTime = GetDate(com.Attributes["date-created"])
					};

					if (!string.IsNullOrEmpty(GetAttributeValue<string>(com.Attributes["url"])))
						c.Website = (GetAttributeValue<string>(com.Attributes["url"]));

					foreach (XmlNode comNode in com.ChildNodes)
					{
						if (comNode.Name == "title")
						{
							c.Content = comNode.InnerText;
						}
					}

					c.Email = c.Content.Contains("pingback", StringComparison.InvariantCultureIgnoreCase) ? "pingback" : "trackback";
					c.Author = c.Email;

					if (blogX.Comments == null)
						blogX.Comments = new List<ExtendedComment>();

					blogX.Comments.Add(c);
				}
			}
		}


		private async Task ImportPostsAsync(BlogMLBlog blog)
		{
			_logger.LogInformation("Start importing posts...");

			foreach (var post in blog.Posts)
			{
				BlogMLPost p = post;

				blogsExtended.Where(b => b.PostUrl == p.PostUrl).FirstOrDefault().BlogPost = post;
			}

			var allPost = blogsExtended.OrderBy(t => t.BlogPost.DateCreated).ToList();

			foreach (BlogMlExtendedPost extPost in allPost)
			{
				if (extPost.BlogPost.PostType == BlogPostTypes.Normal)
				{
					try
					{
						BlogMlExtendedPost post = extPost;

						if (extPost.BlogPost.Categories.Count > 0)
						{
							for (var i = 0; i < extPost.BlogPost.Categories.Count; i++)
							{
								int i2 = i;
								var cId = GetOrAddId("category", post.BlogPost.Categories[i2].Ref);

								var category = categoryLookup.FirstOrDefault(t => t.Id == cId);

								if (category != null)
								{
									if (extPost.Categories == null)
										extPost.Categories = new List<Category>();

									extPost.Categories.Add(category);
								}
							}
						}

						if (await ImportPostAsync(extPost))
						{
							PostCount++;
						}
						else
						{
							_logger.LogInformation("Post '{0}' has been skipped", extPost.BlogPost.Title);
						}
					}
					catch (Exception ex)
					{
						_logger.LogInformation("BlogReader.LoadBlogPosts: " + ex);
					}
				}
				else if (extPost.BlogPost.PostType == BlogPostTypes.Article)
				{
					try
					{
						BlogMlExtendedPost post = extPost;

						if (await ImportPageAsync(extPost))
						{
							PageCount++;
						}
						else
						{
							_logger.LogInformation("Post '{0}' has been skipped", extPost.BlogPost.Title);
						}
					}
					catch (Exception ex)
					{
						_logger.LogInformation("BlogReader.LoadBlogPosts: " + ex);
					}
				}
			}
		}

		private async Task<bool> ImportPostAsync(BlogMlExtendedPost extPost)
		{
			var p = new Post
			{
				Title = extPost.BlogPost.Title,
				CreationTime = extPost.BlogPost.DateCreated,
				PublishedTime = extPost.BlogPost.DateCreated,
				LastModificationTime = extPost.BlogPost.DateModified,
				Content = extPost.BlogPost.Content.UncodedText,
				Description = extPost.BlogPost.Excerpt.UncodedText,
				IsDraft = !extPost.BlogPost.Approved,
				ViewsCount = GetValue(extPost.BlogPost.Views),
				UserId = _user.Id,
				EnableComment = true,
			};

			if (extPost.BlogPost.HasExcerpt)
				p.Description = extPost.BlogPost.Excerpt.UncodedText;

			if (!string.IsNullOrEmpty(extPost.PostUrl))
			{
				// looking for a Slug with patterns such as:
				//    /some-slug.aspx
				//    /some-slug.html
				//    /some-slug
				//
				Match slugMatch = Regex.Match(extPost.PostUrl, @"/([^/\.]+)(?:$|\.[\w]{1,10}$)", RegexOptions.IgnoreCase);
				if (slugMatch.Success)
					p.Slug = slugMatch.Groups[1].Value.Trim();
			}

			if (string.IsNullOrEmpty(p.Slug))
				p.Slug = await _slugService.NormalarAsync(p.Title);

			// skip if exists
			if (await _postService.FindBySlugAsync(p.Slug) != null)
				return false;

			if (extPost.Categories != null && extPost.Categories.Count > 0)
				foreach (var item in extPost.Categories)
				{
					p.Categories.Add(new PostCategory() { CategoryId = item.Id, PostId = item.Id });
				}


			if (extPost.Tags != null && extPost.Tags.Count > 0)
			{
				foreach (var tagName in extPost.Tags)
				{
					var tag = await _tagsService.GetOrCreateAsync(tagName);
					p.Tags.Add(new PostTags() { PostId = p.Id, TagsId = tag.Id });
				}
			}

			if (await _postService.FindBySlugAsync(p.Slug) != null)
				return false;

			await _postService.InsertAsync(p);

			await ImportCommentAsync(extPost, p);

			return true;
		}

		private async Task<bool> ImportPageAsync(BlogMlExtendedPost extPost)
		{
			var p = new Page
			{
				Title = extPost.BlogPost.Title,
				CreationTime = extPost.BlogPost.DateCreated,
				LastModificationTime = extPost.BlogPost.DateModified,
				Content = extPost.BlogPost.Content.UncodedText,
				Description = extPost.BlogPost.Excerpt.UncodedText,
				Published = extPost.BlogPost.Approved,
				DisplayOrder = 1,
			};

			if (!string.IsNullOrEmpty(extPost.PostUrl))
			{
				// looking for a Slug with patterns such as:
				//    /some-slug.aspx
				//    /some-slug.html
				//    /some-slug
				//
				Match slugMatch = Regex.Match(extPost.PostUrl, @"/([^/\.]+)(?:$|\.[\w]{1,10}$)", RegexOptions.IgnoreCase);
				if (slugMatch.Success)
					p.Slug = slugMatch.Groups[1].Value.Trim();
			}

			if (string.IsNullOrEmpty(p.Slug))
				p.Slug = await _slugService.NormalarAsync(p.Title);

			// skip if exists
			if (await _pageService.FindBySlugAsync(p.Slug) != null)
				return false;

			await _pageService.InsertAsync(p);

			return true;
		}

		private async Task ImportCommentAsync(BlogMlExtendedPost extPost, Post post)
		{
			if (extPost.Comments?.Any() == true)
			{
				foreach (var extComment in extPost.Comments)
				{
					var comment = new Comment()
					{
						Author = extComment.Author,
						Content = extComment.Content,
						CreationTime = extComment.CreationTime,
						Email = extComment.Email,
						IP = extComment.IP,
						IsApproved = extComment.IsApproved,
						IsDeleted = extComment.IsDeleted,
						IsSpam = extComment.IsSpam,
						LastModificationTime = extComment.LastModificationTime,
						ParentId = extComment.ParentId,
						Website = extComment.Author,
					};

					// valiate
					if (comment.Author?.Length >= 32) comment.Author = comment.Author.Substring(0, 32);
					if (comment.Email?.Length >= 128) comment.Email = comment.Email.Substring(0, 128);

					comment.PostId = post.Id;
					comment.Country = await _iPAddressService.GetIPLocationAsync(comment.IP);

					if (!string.IsNullOrWhiteSpace(extComment.ParentCommentId))
						comment.ParentId = GetOrAddId("command", extComment.ParentCommentId, comment.Id);

					await _commentService.InsertAsync(comment);

					GetOrAddId("command", extComment.CommentId, comment.Id);
				}

				post.CommentsCount = extPost.Comments.Count;

				await _postService.UpdateAsync(post);
			}
		}

		/// <summary>
		/// Extended BlogML post
		/// </summary>
		public class BlogMlExtendedPost
		{
			/// <summary>
			/// Gets or sets blog post
			/// </summary>
			public BlogMLPost BlogPost { get; set; }

			/// <summary>
			/// Gets or sets post URL
			/// </summary>
			public string PostUrl { get; set; }

			/// <summary>
			/// Gets or sets post tags
			/// </summary>
			public List<string> Tags { get; set; }

			/// <summary>
			/// Gets or sets post categories
			/// </summary>
			public List<Category> Categories { get; set; }

			/// <summary>
			/// Post comments
			/// </summary>
			public List<ExtendedComment> Comments { get; set; }
		}

		public class ExtendedComment : Comment
		{
			public string CommentId { get; set; }
			public string ParentCommentId { get; set; }
		}
	}
}
