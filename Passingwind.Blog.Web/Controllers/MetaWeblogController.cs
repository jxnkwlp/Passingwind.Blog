using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using Passingwind.Blog.MetaWeblog;
using Microsoft.Extensions.Logging;

namespace Passingwind.Blog.Web.Controllers
{
    public class MetaWeblogController : Controller
    {
        private readonly PageManager _pageManager;
        private readonly PostManager _postManager;
        private readonly CategoryManager _categoryManager;
        private readonly TagsManager _tagsManager;
        private readonly CommentManager _commentManager;

        private readonly UserManager _userManager;

        private readonly SettingManager _settingsManager;

        private readonly BasicSettings _basicSettings;
        private readonly CommentsSettings _commentsSettings;

        //private readonly PermissionChecker _permissionChecker;

        //private readonly IFileManager _fileManager;

        private readonly ILogger _logger;

        public MetaWeblogController(LoggerFactory loggerFactory, PostManager postManager, PageManager pageManager, CategoryManager categoryManager, TagsManager tagsManager, SettingManager settingsManager, CommentManager commentManager, UserManager userManager, BasicSettings basicSettings, CommentsSettings commentsSettings)
        {
            this._pageManager = pageManager;
            this._postManager = postManager;
            this._categoryManager = categoryManager;
            this._tagsManager = tagsManager;
            this._commentManager = commentManager;
            this._settingsManager = settingsManager;
            this._userManager = userManager;

            //this._permissionChecker = permissionChecker;
            //this._fileManager = fileManager;

            this._basicSettings = basicSettings;
            this._commentsSettings = commentsSettings;

            _logger = loggerFactory.CreateLogger<MetaWeblogController>();
        }

        [Route("api/metaweblog", Name = RouteNames.Metaweblog)]
        public async Task<IActionResult> Index()
        {
            var resultXml = await ProcessRequest();

            _logger.LogDebug(resultXml);

            return Content(resultXml, "text/xml");
        }

        protected string GetRequest(Stream stream)
        {
            var buffer = new byte[stream.Length];

            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);

            var xmlString = Encoding.UTF8.GetString(buffer);

            return xmlString;
        }

        protected async Task<string> ProcessRequest()
        {
            string responseXml = string.Empty;

            try
            {
                var rootUrl = Request.Host.ToUriComponent();

                var inputXml = GetRequest(Request.Body);

                _logger.LogDebug(inputXml);

                var input = new XMLRPCRequest(inputXml);

                var output = new XMLRPCResponse(input.MethodName);

                var user = GetVerifyUserAsync(input.UserName, input.Password);

                if (user == null)
                {
                    throw new MetaWeblogException("11", "User authentication failed");
                }

                switch (input.MethodName)
                {
                    case "metaWeblog.newPost":
                        output.PostID = await this.NewPost(
                            input.BlogID, input.UserName, input.Password, input.Post, input.Publish);
                        break;
                    case "metaWeblog.editPost":
                        output.Completed = await this.EditPost(
                            input.PostID, input.UserName, input.Password, input.Post, input.Publish);
                        break;
                    case "metaWeblog.getPost":
                        output.Post = await this.GetPost(input.PostID, input.UserName, input.Password);
                        break;
                    case "metaWeblog.newMediaObject":
                        output.MediaInfo = await this.NewMediaObject(input.BlogID, input.UserName, input.Password, input.MediaObject);
                        break;
                    case "metaWeblog.getCategories":
                        output.Categories = await this.GetCategories(input.BlogID, input.UserName, input.Password, rootUrl);
                        break;
                    case "metaWeblog.getRecentPosts":
                        output.Posts = await this.GetRecentPosts(input.BlogID, input.UserName, input.Password, input.NumberOfPosts);
                        break;
                    case "blogger.getUsersBlogs":
                    case "metaWeblog.getUsersBlogs":
                        output.Blogs = this.GetUserBlogs(input.AppKey, input.UserName, input.Password, rootUrl);
                        break;
                    case "blogger.deletePost":
                        output.Completed = await this.DeletePost(
                            input.AppKey, input.PostID, input.UserName, input.Password, input.Publish);
                        break;
                    case "blogger.getUserInfo":
                        // Not implemented.  Not planned.
                        throw new MetaWeblogException("10", "The method GetUserInfo is not implemented.");
                    case "wp.newPage":
                        output.PageID = await this.NewPage(input.BlogID, input.UserName, input.Password, input.Page, input.Publish);
                        break;
                    case "wp.getPageList":
                    case "wp.getPages":
                        output.Pages = await this.GetPages(input.BlogID, input.UserName, input.Password);
                        break;
                    case "wp.getPage":
                        output.Page = await this.GetPage(input.BlogID, input.PageID, input.UserName, input.Password);
                        break;
                    case "wp.editPage":
                        output.Completed = await this.EditPage(input.BlogID, input.PageID, input.UserName, input.Password, input.Page, input.Publish);
                        break;
                    case "wp.deletePage":
                        output.Completed = await this.DeletePage(input.BlogID, input.PageID, input.UserName, input.Password);
                        break;
                    case "wp.getAuthors":
                        output.Authors = await this.GetAuthors(input.BlogID, input.UserName, input.Password);
                        break;
                    case "wp.getTags":
                        output.Keywords = await this.GetKeywords(input.BlogID, input.UserName, input.Password);
                        break;
                }

                responseXml = output.ResponseXml();
            }
            catch (MetaWeblogException mex)
            {
                var output = new XMLRPCResponse("fault");
                var fault = new MWAFault { faultCode = mex.Code, faultString = mex.Message };
                output.Fault = fault;

                responseXml = output.ResponseXml();
            }
            catch (Exception ex)
            {
                var output = new XMLRPCResponse("fault");
                var fault = new MWAFault { faultCode = "0", faultString = ex.Message };
                output.Fault = fault;

                responseXml = output.ResponseXml();
            }

            return responseXml;
        }

        protected async Task<User> GetVerifyUserAsync(string userName, string password)
        {
            var user = await _userManager.FindByEmailAsync(userName);

            if (user != null)
            {
                if (await _userManager.VerifyPasswordAsync(user, password))
                    return user;
            }

            return null;
        }

        #region Methods

        /// <summary>
        /// Deletes the page.
        /// </summary>
        /// <param name="blogId">
        /// The blog id.
        /// </param>
        /// <param name="pageId">
        /// The page id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The delete page.
        /// </returns>
        /// <exception cref="MetaWeblogException">
        /// </exception>
        internal async Task<bool> DeletePage(string blogId, string pageId, string userName, string password)
        {
            try
            {
                var user = GetVerifyUserAsync(userName, password);

                if (user == null)
                {
                    throw new MetaWeblogException("11", "User authentication failed");
                }

                //if (!_permissionChecker.IsValid(user, PermissionKeys.PageDelete))
                //{
                //    throw new MetaWeblogException("11", "User authentication failed");
                //}

                await _pageManager.DeleteByIdAsync(pageId);

            }
            catch (Exception ex)
            {
                throw new MetaWeblogException("15", $"DeletePage failed.  Error: {ex.Message}");
            }

            return true;
        }

        /// <summary>
        /// blogger.deletePost method
        /// </summary>
        /// <param name="appKey">
        /// Key from application.  Outdated methodology that has no use here.
        /// </param>
        /// <param name="postId">
        /// post guid in string format
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <param name="publish">
        /// mark as published?
        /// </param>
        /// <returns>
        /// Whether deletion was successful or not.
        /// </returns>
        internal async Task<bool> DeletePost(string appKey, string postId, string userName, string password, bool publish)
        {
            try
            {
                var user = GetVerifyUserAsync(userName, password);

                //if (!_permissionChecker.IsValid(user, PermissionKeys.PostDelete))
                //{
                //    throw new MetaWeblogException("11", "User authentication failed");
                //}

                await _postManager.DeleteByIdAsync(postId);
            }
            catch (Exception ex)
            {
                throw new MetaWeblogException("12", $"DeletePost failed.  Error: {ex.Message}");
            }

            return true;
        }

        /// <summary>
        /// Edits the page.
        /// </summary>
        /// <param name="blogId">
        /// The blog id.
        /// </param>
        /// <param name="pageId">
        /// The page id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="mwaPage">
        /// The m page.
        /// </param>
        /// <param name="publish">
        /// The publish.
        /// </param>
        /// <returns>
        /// The edit page.
        /// </returns>
        internal async Task<bool> EditPage(string blogId, string pageId, string userName, string password, MWAPage mwaPage, bool publish)
        {
            var user = GetVerifyUserAsync(userName, password);

            //if (!_permissionChecker.IsValid(user, PermissionKeys.PageEdit))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            var page = await _pageManager.FindByIdAsync(pageId);

            page.Title = mwaPage.title;
            page.Content = mwaPage.description;
            page.Keywords = mwaPage.mt_keywords;

            page.Published = publish;

            page.ParentId = mwaPage.pageParentID;

            await _pageManager.UpdateAsync(page);

            return true;
        }

        /// <summary>
        /// metaWeblog.editPost method
        /// </summary>
        /// <param name="postId">
        /// post guid in string format
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <param name="sentPost">
        /// struct with post details
        /// </param>
        /// <param name="publish">
        /// mark as published?
        /// </param>
        /// <returns>
        /// 1 if successful
        /// </returns>
        internal async Task<bool> EditPost(string postId, string userName, string password, MWAPost sentPost, bool publish)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.PostEdit))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            var post = await _postManager.FindByIdAsync(postId);

            string author = String.IsNullOrEmpty(sentPost.author) ? userName : sentPost.author;

            var authorUser = await _userManager.FindByEmailAsync(author);
            if (authorUser != null)
            {
                post.UserId = authorUser.Id;
            }

            post.Title = sentPost.title;
            post.Content = sentPost.description;
            post.IsDraft = !publish;
            post.Slug = sentPost.slug;
            post.Description = sentPost.excerpt;
            if (sentPost.postDate != new DateTime())
                post.PublishedTime = sentPost.postDate;

            if (sentPost.commentPolicy != string.Empty)
            {
                post.EnableComment = sentPost.commentPolicy == "1";
            }

            post.Categories.Clear();
            foreach (var item in sentPost.categories.Where(c => c != null && c.Trim() != string.Empty))
            {
                Category cat;
                if (LookupCategoryGuidByName(item, out cat))
                {
                    post.Categories.Add(new PostCategory() { CategoryId = cat.Id, PostId = post.Id });
                }
                else
                {
                    // Allowing new categories to be added.  (This breaks spec, but is supported via WLW)
                    var newcat = new Category()
                    {
                        Name = item,
                        DisplayOrder = 1,
                    };

                    post.Categories.Add(new PostCategory() { Category = newcat, PostId = post.Id });
                }
            }

            post.Tags.Clear();
            foreach (var item in sentPost.tags.Where(item => item != null && item.Trim() != string.Empty))
            {
                var tag = await _tagsManager.CreateOrUpdateAsync(item);
                post.Tags.Add(new PostTags() { TagsId = tag.Id, PostId = post.Id });
            }

            await _postManager.UpdateAsync(post);

            return true;
        }

        /// <summary>
        /// Gets authors.
        /// </summary>
        /// <param name="blogId">
        /// The blog id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// A list of authors.
        /// </returns>
        internal async Task<List<MWAAuthor>> GetAuthors(string blogId, string userName, string password)
        {
            var authors = new List<MWAAuthor>();

            var currentUser = await GetVerifyUserAsync(userName, password);

            if ((await _userManager.GetRolesAsync(currentUser)).Contains(Role.AdministratorName))
            {
                var users = _userManager.GetQueryable().ToList().Select(u => new MWAAuthor
                {
                    user_id = u.UserName,
                    user_login = u.UserName,
                    display_name = u.UserName,
                    user_email = u.Email,
                    meta_value = string.Empty
                });

                authors.AddRange(users);

            }
            else
            {
                // If not able to administer others, just add that user to the options.

                authors.Add(new MWAAuthor
                {
                    user_id = currentUser.UserName,
                    user_login = currentUser.UserName,
                    display_name = currentUser.UserName,
                    user_email = currentUser.Email,
                    meta_value = string.Empty
                });
            }

            return authors;
        }

        /// <summary>
        /// metaWeblog.getCategories method
        /// </summary>
        /// <param name="blogId">
        /// always 1000 in BlogEngine since it is a singlar blog instance
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <param name="rootUrl">
        /// The root URL.
        /// </param>
        /// <returns>
        /// array of category structs
        /// </returns>
        internal async Task<List<MWACategory>> GetCategories(string blogId, string userName, string password, string rootUrl)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.PostCreate))
            //{
            //    return new List<MWACategory>();
            //}

            return _categoryManager.GetQueryable().ToList().Select(cat => new MWACategory
            {
                title = cat.Name,
                description = cat.Name,
                htmlUrl = Url.RouteUrl(RouteNames.Category, new { name = cat.Name }, Request.Scheme),
                rssUrl = Url.RouteUrl(RouteNames.SyndicationCategory, new { name = cat.Name }, Request.Scheme)
            }).ToList();
        }

        /// <summary>
        /// wp.getTags method
        /// </summary>
        /// <param name="blogId">The blog id.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>list of tags</returns>
        internal async Task<List<string>> GetKeywords(string blogId, string userName, string password)
        {
            var keywords = new List<string>();

            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.PostCreate))
            //{
            //    return keywords;
            //}

            keywords.AddRange(_tagsManager.GetQueryable().Select(t => t.Name).ToList());

            return keywords;
        }

        /// <summary>
        /// wp.getPage method
        /// </summary>
        /// <param name="blogId">
        /// blogID in string format
        /// </param>
        /// <param name="pageId">
        /// page guid in string format
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <returns>
        /// struct with post details
        /// </returns>
        internal async Task<MWAPage> GetPage(string blogId, string pageId, string userName, string password)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.Pages))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            var sendPage = new MWAPage();

            var page = await _pageManager.FindByIdAsync(pageId);

            sendPage.pageID = page.Id.ToString();
            sendPage.title = page.Title;
            sendPage.description = page.Content;
            sendPage.mt_keywords = page.Keywords;
            sendPage.pageDate = page.CreationTime;
            sendPage.link = Url.RouteUrl(RouteNames.Page, new { id = page.Id }, Request.Scheme);
            sendPage.mt_convert_breaks = "__default__";
            sendPage.pageParentID = page.ParentId.ToString();

            return sendPage;
        }

        /// <summary>
        /// wp.getPages method
        /// </summary>
        /// <param name="blogId">
        /// blogID in string format
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <returns>
        /// a list of pages
        /// </returns>
        internal async Task<List<MWAPage>> GetPages(string blogId, string userName, string password)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.Pages))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            return _pageManager.GetQueryable().Where(t => t.Published).ToList()
                .Select(page => new MWAPage
                {
                    pageID = page.Id.ToString(),
                    title = page.Title,
                    description = page.Content,
                    mt_keywords = page.Keywords,
                    pageDate = page.CreationTime,
                    link = Url.RouteUrl(RouteNames.Page, new { id = page.Id }, Request.Scheme),
                    mt_convert_breaks = "__default__",
                    pageParentID = page.ParentId.ToString()
                }).ToList();
        }

        /// <summary>
        /// metaWeblog.getPost method
        /// </summary>
        /// <param name="postId">
        /// post guid in string format
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <returns>
        /// struct with post details
        /// </returns>
        internal async Task<MWAPost> GetPost(string postId, string userName, string password)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.Posts))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            var sendPost = new MWAPost();
            var post = await _postManager.FindByIdAsync(postId);


            sendPost.postID = post.Id.ToString();
            sendPost.postDate = post.PublishedTime;
            sendPost.title = post.Title;
            sendPost.description = post.Content;
            sendPost.link = Url.RouteUrl(RouteNames.Post, new { id = post.Id }, Request.Scheme);
            sendPost.slug = post.Slug;
            sendPost.excerpt = post.Description;
            sendPost.commentPolicy = post.EnableComment ? "1" : "0";
            sendPost.publish = !post.IsDraft;

            sendPost.categories = post.Categories.Select(t => t.Category.Name).ToList();

            sendPost.tags = post.Tags.Select(t => t.Tags.Name).ToList();

            return sendPost;
        }

        /// <summary>
        /// metaWeblog.getRecentPosts method
        /// </summary>
        /// <param name="blogId">
        /// always 1000 in BlogEngine since it is a singlar blog instance
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <param name="numberOfPosts">
        /// number of posts to return
        /// </param>
        /// <returns>
        /// array of post structs
        /// </returns>
        internal async Task<List<MWAPost>> GetRecentPosts(string blogId, string userName, string password, int numberOfPosts)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.Posts))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            var sendPosts = new List<MWAPost>();
            var posts = _postManager.GetQueryable().Where(t => !t.IsDraft).Take(numberOfPosts).ToList();

            foreach (var post in posts)
            {
                var tempPost = new MWAPost
                {
                    postID = post.Id.ToString(),
                    postDate = post.PublishedTime,
                    title = post.Title,
                    description = post.Content,
                    link = Url.RouteUrl(RouteNames.Post, new { id = post.Id }, Request.Scheme),
                    slug = post.Slug,
                    excerpt = post.Description,
                    commentPolicy = post.EnableComment ? string.Empty : "0",
                    publish = !post.IsDraft,
                };

                tempPost.categories = post.Categories.Select(t => t.Category.Name).ToList();

                tempPost.tags = post.Tags.Select(t => t.Tags.Name).ToList();

                sendPosts.Add(tempPost);
            }

            return sendPosts;
        }

        /// <summary>
        /// blogger.getUsersBlogs method
        /// </summary>
        /// <param name="appKey">
        /// Key from application.  Outdated methodology that has no use here.
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <param name="rootUrl">
        /// The root URL.
        /// </param>
        /// <returns>
        /// array of blog structs
        /// </returns>
        internal List<MWABlogInfo> GetUserBlogs(string appKey, string userName, string password, string rootUrl)
        {
            var blogs = new List<MWABlogInfo>();

            var temp = new MWABlogInfo
            {
                url = rootUrl,
                blogID = "1000",
                blogName = _basicSettings.Title,
            };

            blogs.Add(temp);

            return blogs;
        }

        /// <summary>
        /// metaWeblog.newMediaObject method
        /// </summary>
        /// <param name="blogId">
        /// always 1000 in BlogEngine since it is a singlar blog instance
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <param name="mediaObject">
        /// struct with media details
        /// </param>
        /// <param name="request">
        /// The HTTP request.
        /// </param>
        /// <returns>
        /// struct with url to media
        /// </returns>
        internal async Task<MWAMediaInfo> NewMediaObject(string blogId, string userName, string password, MWAMediaObject mediaObject)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.PageCreate, PermissionKeys.PostCreate))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            // var path = _fileManager.Save(mediaObject.name, mediaObject.bits);

            var mediaInfo = new MWAMediaInfo()
            {
                // url = VirtualPathUtility.ToAbsolute(path, Request.ApplicationPath),
            };

            return mediaInfo;

        }

        /// <summary>
        /// wp.newPage method
        /// </summary>
        /// <param name="blogId">blogID in string format</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="mwaPage">The mwa page.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns>The new page.</returns>
        internal async Task<string> NewPage(string blogId, string userName, string password, MWAPage mwaPage, bool publish)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.PageCreate))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            var page = new Page
            {
                Title = mwaPage.title,
                Content = mwaPage.description,
                Keywords = mwaPage.mt_keywords,
                Published = publish,
            };

            if (mwaPage.pageDate != new DateTime())
            {
                page.CreationTime = mwaPage.pageDate;
            }

            page.ParentId = mwaPage.pageParentID;

            await _pageManager.CreateAsync(page);

            return page.Id.ToString();
        }

        /// <summary>
        /// metaWeblog.newPost method
        /// </summary>
        /// <param name="blogId">
        /// always 1000 in BlogEngine since it is a singlar blog instance
        /// </param>
        /// <param name="userName">
        /// login username
        /// </param>
        /// <param name="password">
        /// login password
        /// </param>
        /// <param name="sentPost">
        /// struct with post details
        /// </param>
        /// <param name="publish">
        /// mark as published?
        /// </param>
        /// <returns>
        /// postID as string
        /// </returns>
        internal async Task<string> NewPost(string blogId, string userName, string password, MWAPost sentPost, bool publish)
        {
            var currentUser = await GetVerifyUserAsync(userName, password);
            //if (!_permissionChecker.IsValid(currentUser, PermissionKeys.PostCreate))
            //{
            //    throw new MetaWeblogException("11", "User authentication failed");
            //}

            var post = new Post
            {
                UserId = currentUser.Id,
                Title = sentPost.title,
                Content = sentPost.description,
                IsDraft = !publish,
                Slug = sentPost.slug,
                Description = sentPost.excerpt,
            };

            string authorName = String.IsNullOrEmpty(sentPost.author) ? userName : sentPost.author;

            var user = await _userManager.FindByEmailAsync(authorName);
            if (user != null)
            {
                post.UserId = user.Id;
            }

            if (sentPost.commentPolicy != string.Empty)
            {
                post.EnableComment = sentPost.commentPolicy == "1";
            }

            post.Categories.Clear();
            foreach (var item in sentPost.categories.Where(c => c != null && c.Trim() != string.Empty))
            {
                Category cat;
                if (LookupCategoryGuidByName(item, out cat))
                {
                    post.Categories.Add(new PostCategory() { CategoryId = cat.Id, PostId = post.Id });
                }
                else
                {
                    // Allowing new categories to be added.  (This breaks spec, but is supported via WLW)
                    var newcat = new Category()
                    {
                        Name = item,
                        DisplayOrder = 1,
                    };

                    post.Categories.Add(new PostCategory() { Category = newcat, PostId = post.Id });
                }
            }

            post.Tags.Clear();
            foreach (var item in sentPost.tags.Where(item => item != null && item.Trim() != string.Empty))
            {
                var tag = await _tagsManager.CreateOrUpdateAsync(item);
                post.Tags.Add(new PostTags() { TagsId = tag.Id, PostId = post.Id });
            }

            post.CreationTime = sentPost.postDate == new DateTime() ? DateTime.Now : sentPost.postDate;

            await _postManager.CreateAsync(post);

            return post.Id.ToString();
        }

        /// <summary>
        /// Returns Category Guid from Category name.
        /// </summary>
        /// <remarks>
        /// Reverse dictionary lookups are ugly.
        /// </remarks>
        /// <param name="name">
        /// The category name.
        /// </param>
        /// <param name="cat">
        /// The category.
        /// </param>
        /// <returns>
        /// Whether the category was found or not.
        /// </returns>
        private bool LookupCategoryGuidByName(string name, out Category cat)
        {
            var categories = _categoryManager.GetQueryable().ToList();

            cat = new Category();

            foreach (var item in categories.Where(item => item.Name == name))
            {
                cat = item;
                return true;
            }

            return false;
        }

        #endregion
    }
}
