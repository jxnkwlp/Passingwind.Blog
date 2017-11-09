using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using BlogML;
using BlogML.Xml;
using Microsoft.Extensions.Logging;

namespace Passingwind.Blog.BlogML
{
    public class BlogMLImporter
    {
        private readonly PostManager _postManager;
        private readonly PageManager _pageManager;
        private readonly CategoryManager _categoryManager;
        private readonly TagsManager _tagsManager;
        private readonly CommentManager _commentManager;

        private readonly ILogger _logger;

        public BlogMLImporter(ILoggerFactory loggerFactory, PostManager postManager, PageManager pageManager, TagsManager tagsManager, CategoryManager categoryManager, CommentManager commentManager)
        {
            this._postManager = postManager;
            this._pageManager = pageManager;
            this._categoryManager = categoryManager;
            this._tagsManager = tagsManager;
            this._commentManager = commentManager;


            this._logger = loggerFactory.CreateLogger<BlogMLImporter>();
        }

        public bool ApprovedCommentsOnly { get; set; }

        public string Author { get; set; }

        public string Message { get; set; }

        public bool RemoveDuplicates { get; set; }

        private User _author;

        public int PostCount { get; set; }

        public string XmlData
        {
            set
            {
                xmlData = value;
            }
        }

        private List<Category> categoryLookup = new List<Category>();
        private List<BlogMlExtendedPost> blogsExtended = new List<BlogMlExtendedPost>();
        private List<BlogMlExtendedPost> blogsPages = new List<BlogMlExtendedPost>();
        private string xmlData = string.Empty;


        private XmlReader XmlReader
        {
            get
            {
                var byteArray = Encoding.UTF8.GetBytes(this.xmlData);
                var stream = new MemoryStream(byteArray);
                return XmlReader.Create(stream);
            }
        }

        public async Task<bool> ImportAsync(User author)
        {
            _author = author;

            Message = string.Empty;

            var blog = new BlogMLBlog();

            try
            {
                blog = BlogMLSerializer.Deserialize(XmlReader);
            }
            catch (Exception ex)
            {
                Message = string.Format("BlogReader.Import: BlogML could not load with 2.0 specs. {0}", ex.Message);

                _logger.LogError(Message);

                return false;
            }

            if (blog.Authors.Count > 0)
            {
                var firstAuthor = blog.Authors[0];
            }

            try
            {
                LoadFromXmlDocument();

                await LoadBlogCategories(blog);

                LoadBlogExtendedPosts(blog);

                await LoadBlogPosts();

                Message = string.Format("Imported {0} new posts", PostCount);
            }
            catch (Exception ex)
            {
                Message = string.Format("BlogReader.Import: {0}", ex.Message);

                _logger.LogError(Message);

                return false;
            }

            return true;
        }


        #region Methods

        private T GetAttributeValue<T>(XmlAttribute attr)
        {
            if (attr == null)
                return default(T);

            return (T)Convert.ChangeType(attr.Value, typeof(T));
        }

        private Dictionary<string, Dictionary<string, Guid>> _substitueGuids;
        private Guid GetGuid(string type, string value)
        {
            value = (value ?? string.Empty).Trim();

            if (value.Length == 36)
            {
                return new Guid(value);
            }

            if (_substitueGuids == null)
            {
                _substitueGuids = new Dictionary<string, Dictionary<string, Guid>>(StringComparer.OrdinalIgnoreCase);
            }

            if (!_substitueGuids.ContainsKey(type))
                _substitueGuids.Add(type, new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase));

            if (!_substitueGuids[type].ContainsKey(value))
                _substitueGuids[type].Add(value, Guid.NewGuid());

            return _substitueGuids[type][value];
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
            Uri uri;
            if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri))
                return uri;

            return null;
        }

        private int GetValue(object value)
        {
            if (value == null)
                return 0;

            int i = 0;
            if (int.TryParse(value.ToString(), out i))
                return i;

            return 0;
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
                    var c = new Comment
                    {
                        // Id = GetGuid("comment", GetAttributeValue<string>(com.Attributes["id"])).ToString(),
                        Author = GetAttributeValue<string>(com.Attributes["user-name"]),
                        Email = GetAttributeValue<string>(com.Attributes["user-email"]),
                        IP = GetAttributeValue<string>(com.Attributes["user-ip"]),
                        CreationTime = GetDate(com.Attributes["date-created"]),
                    };

                    var parentid = GetAttributeValue<string>(com.Attributes["parentid"]);
                    if (!string.IsNullOrEmpty(parentid))
                        c.ParentId = GetGuid("comment", parentid).ToString();

                    if (!string.IsNullOrEmpty(GetAttributeValue<string>(com.Attributes["user-url"])))
                        c.Website = (GetAttributeValue<string>(com.Attributes["user-url"]));

                    c.IsApproved = GetAttributeValue<bool>(com.Attributes["approved"]);

                    foreach (XmlNode comNode in com.ChildNodes)
                    {
                        if (comNode.Name == "content")
                        {
                            c.Content = comNode.InnerText;
                        }
                    }

                    if (blogX.Comments == null)
                        blogX.Comments = new List<Comment>();

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
                    var c = new Comment
                    {
                        Id = GetGuid("comment", GetAttributeValue<string>(com.Attributes["id"])).ToString(),
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

                    c.Email = c.Content.ToLowerInvariant().Contains("pingback") ? "pingback" : "trackback";
                    c.Author = c.Email;

                    if (blogX.Comments == null)
                        blogX.Comments = new List<Comment>();

                    blogX.Comments.Add(c);
                }
            }
        }

        private async Task LoadBlogCategories(BlogMLBlog blog)
        {
            foreach (var cat in blog.Categories)
            {
                var c = new Category
                {
                    Id = GetGuid("category", cat.ID).ToString(),
                    Name = cat.Title,
                    Description = cat.Description,
                    DisplayOrder = 1,
                };

                c.Slug = c.GetSeName();

                //if (!string.IsNullOrEmpty(cat.ParentRef) && cat.ParentRef != "0")
                //    c.Parent = GetGuid("category", cat.ParentRef);

                if (!_categoryManager.GetQueryable().Any(t => t.Slug == c.Slug))
                {
                    await _categoryManager.CreateAsync(c);
                }
                else
                {
                    c = _categoryManager.GetQueryable().FirstOrDefault(t => t.Slug == c.Slug);
                }

                categoryLookup.Add(c);

            }
        }

        private void LoadBlogExtendedPosts(BlogMLBlog blog)
        {
            foreach (var post in blog.Posts)
            {
                BlogMLPost p = post;

                blogsExtended.Where(b => b.PostUrl == p.PostUrl).FirstOrDefault().BlogPost = post;

                //if (post.PostType == BlogPostTypes.Normal)
                //{
                //    BlogMLPost p = post;

                //    blogsExtended.Where(b => b.PostUrl == p.PostUrl).FirstOrDefault().BlogPost = post;
                //}
                //else if (post.PostType == BlogPostTypes.Article)
                //{
                //    BlogMLPost p = post;

                //    blogsExtended.Where(b => b.PostUrl == p.PostUrl).FirstOrDefault().BlogPost = post;
                //}
            }
        }

        private async Task LoadBlogPosts()
        {
            _logger.LogInformation("BlogReader.LoadBlogPosts: Start importing posts");

            var allPost = blogsExtended.OrderByDescending(t => t.BlogPost.DateCreated).ToList();

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
                                var cId = GetGuid("category", post.BlogPost.Categories[i2].Ref).ToString();

                                foreach (var category in categoryLookup)
                                {
                                    if (category.Id == cId)
                                    {
                                        if (extPost.Categories == null)
                                            extPost.Categories = new List<Category>();

                                        extPost.Categories.Add(category);
                                    }
                                }
                            }
                        }

                        if (await AddPost(extPost))
                        {
                            PostCount++;
                        }
                        else
                        {
                            _logger.LogInformation("Post '{0}' has been skipped" + extPost.BlogPost.Title);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("BlogReader.LoadBlogPosts: " + ex.Message);
                    }
                }
                else if (extPost.BlogPost.PostType == BlogPostTypes.Article)
                {
                    try
                    {
                        BlogMlExtendedPost post = extPost;

                        if (await AddPage(extPost))
                        {
                            PostCount++;
                        }
                        else
                        {
                            _logger.LogInformation("Post '{0}' has been skipped" + extPost.BlogPost.Title);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("BlogReader.LoadBlogPosts: " + ex.Message);
                    }
                }
            }

            _logger.LogInformation(string.Format("BlogReader.LoadBlogPosts: Completed importing {0} posts", PostCount));
        }

        private async Task<bool> AddPost(BlogMlExtendedPost extPost)
        {
            var p = new Post();

            p.Title = extPost.BlogPost.Title;
            p.CreationTime = extPost.BlogPost.DateCreated;
            p.PublishedTime = extPost.BlogPost.DateCreated;
            p.LastModificationTime = extPost.BlogPost.DateModified;
            p.Content = extPost.BlogPost.Content.UncodedText;
            p.Description = extPost.BlogPost.Excerpt.UncodedText;
            p.IsDraft = !extPost.BlogPost.Approved;
            p.ViewsCount = GetValue(extPost.BlogPost.Views);

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
                p.Slug = p.GetSeName();

            // skip if exists
            if (await _postManager.FindBySlugAsync(p.Slug) != null)
                return false;

            if (extPost.BlogPost.Authors != null && extPost.BlogPost.Authors.Count > 0)
            {
                // p.UserId = extPost.BlogPost.Authors[0].Ref;
                p.UserId = _author.Id;
            }


            if (extPost.Categories != null && extPost.Categories.Count > 0)
                foreach (var item in extPost.Categories)
                {
                    p.Categories.Add(new PostCategory() { CategoryId = item.Id, PostId = item.Id });
                }


            if (extPost.Tags != null && extPost.Tags.Count > 0)
            {
                foreach (var item in extPost.Tags)
                {
                    var tag = await _tagsManager.CreateOrUpdateAsync(item);
                    p.Tags.Add(new PostTags() { PostId = p.Id, TagsId = tag.Id });
                }
            }

            await _postManager.CreateAsync(p);

            if (extPost.Comments != null && extPost.Comments.Count > 0)
            {
                foreach (var comment in extPost.Comments)
                {
                    comment.PostId = p.Id;
                    try
                    {
                        await _commentManager.CreateAsync(comment);
                    }
                    catch (Exception)
                    {
                    } 
                }

                p.CommentsCount = extPost.Comments.Count;

                await _postManager.UpdateAsync(p);
            }

            return true;
        }

        private async Task<bool> AddPage(BlogMlExtendedPost extPost)
        {
            var p = new Page();

            p.Title = extPost.BlogPost.Title;
            p.CreationTime = extPost.BlogPost.DateCreated;
            p.LastModificationTime = extPost.BlogPost.DateModified;
            p.Content = extPost.BlogPost.Content.UncodedText;
            p.Description = extPost.BlogPost.Excerpt.UncodedText;
            p.Published = extPost.BlogPost.Approved;

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
                p.Slug = p.GetSeName();

            // skip if exists
            if (await _pageManager.FindBySlugAsync(p.Slug) != null)
                return false;

            await _pageManager.CreateAsync(p);

            return true;
        }
    }

    #endregion

}
