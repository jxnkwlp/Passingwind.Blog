using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Passingwind.Blog.Pager;
using Passingwind.Blog.Web.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Passingwind.Blog.Web.Services;

namespace Passingwind.Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostManager _postManager;
        private readonly CategoryManager _categoryManager;
        private readonly TagsManager _tagManager;
        private readonly PageManager _pageManager;
        private readonly CommentManager _commentManager;
        private readonly UserManager _userManager;
        private readonly IEmailSender _emailSender;

        private readonly BasicSettings _basicSettings;
        private readonly CommentsSettings _commentsSettings;
        private readonly EmailSettings _emailSettings;

        public HomeController(PostManager postManager, CategoryManager categoryManager, TagsManager tagManager, PageManager pageManager, CommentManager commentManager, UserManager userManager, BasicSettings basicSettings, CommentsSettings commentsSettings, EmailSettings emailSettings, IEmailSender emailSender)
        {
            this._postManager = postManager;
            this._categoryManager = categoryManager;
            this._tagManager = tagManager;
            this._pageManager = pageManager;
            this._commentManager = commentManager;
            this._userManager = userManager;

            this._emailSender = emailSender;

            this._basicSettings = basicSettings;
            this._commentsSettings = commentsSettings;
            this._emailSettings = emailSettings;
        }

        #region Post List

        public async Task<IActionResult> Index(int page)
        {
            page = page <= 1 ? 1 : page;

            if (Request.IsCategoryUrl())
            {
                var categoryValue = this.RouteData.Values["name"];

                string categoryName = categoryValue?.ToString();

                if (categoryName != null)
                {
                    var result = await GetPostsByCategory(categoryName, page);

                    if (result.Item1 == null)
                    {
                        return View("NotFound");
                    }

                    await SetTitleAsync(result.Item1.Name, page: page);

                    return View("Posts", result.Item2);
                }
            }
            else if (Request.IsTagsUrl())
            {
                var tagValue = this.RouteData.Values["name"];

                string tagName = tagValue?.ToString();

                if (tagName != null)
                {
                    var result = await GetPostsByTag(tagName, page);

                    if (result.Item1 == null)
                    {
                        return View("NotFound");
                    }

                    await SetTitleAsync(result.Item1.Name, page: page);

                    return View("Posts", result.Item2);
                }
            }
            else if (Request.IsAuthorUrl())
            {
                var value = this.RouteData.Values["username"];

                string username = value?.ToString();

                if (!string.IsNullOrEmpty(username))
                {
                    var result = await GetPostsByAuthor(username, page);

                    if (result.user == null)
                    {
                        return View("NotFound");
                    }

                    await SetTitleAsync(result.user.DisplayName, page: page);

                    return View("Posts", result.post);
                }

            }
            else if (Request.IsMonthListUrl())
            {
                var yearValue = this.RouteData.Values["year"];
                var monthValue = this.RouteData.Values["month"];

                if (yearValue != null && monthValue != null)
                {
                    int year = 0;
                    int month = 0;

                    if (int.TryParse(yearValue.ToString(), out year))
                    {
                        if (int.TryParse(monthValue.ToString(), out month))
                        {
                            var result = await GetPostsByMonth(year, month, page);

                            await SetTitleAsync(result.Item1.ToString("yyyy/MM"), page: page);

                            return View("Posts", result.Item2);
                        }
                    }

                    return View("NotFound");
                }
            }
            else if (Request.IsSearchUrl())
            {
                string searchTerm = Request.Query["q"];

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var result = await GetPostsBySearch(searchTerm, page);

                    await SetTitleAsync(searchTerm, page: page);

                    return View("Posts", result);
                }
            }

            await SetTitleAsync("");

            return View("Posts", await GetPosts(page));
        }

        private async Task<Tuple<Tags, IPagedList<PostViewModel>>> GetPostsByTag(string tagName, int page)
        {
            var query = _postManager.GetQueryable();

            var tag = await _tagManager.FindByNameAsync(tagName);
            if (tag != null)
            {
                query = query.Where(t => t.Tags.Any(c => c.TagsId == tag.Id));
            }

            var result = await GetPosts(query, page);

            return new Tuple<Tags, IPagedList<PostViewModel>>(tag, result);
        }

        private async Task<Tuple<Category, IPagedList<PostViewModel>>> GetPostsByCategory(string categoryName, int page)
        {
            var query = _postManager.GetQueryable();

            var category = await _categoryManager.GetBySlugAsync(categoryName);
            if (category != null)
            {
                query = query.Where(t => t.Categories.Any(c => c.CategoryId == category.Id));
            }

            var result = await GetPosts(query, page);

            return new Tuple<Category, IPagedList<PostViewModel>>(category, result);
        }

        private async Task<(User user, IPagedList<PostViewModel> post)> GetPostsByAuthor(string username, int page)
        {
            var query = _postManager.GetQueryable();

            var user = await _userManager.FindByNameAsync(username);

            query = query.Where(t => string.Equals(t.User.UserName, username, StringComparison.CurrentCultureIgnoreCase));

            var result = await GetPosts(query, page);

            return new ValueTuple<User, IPagedList<PostViewModel>>(user, result);
        }

        private async Task<Tuple<DateTime, IPagedList<PostViewModel>>> GetPostsByMonth(int year, int month, int page)
        {
            var query = _postManager.GetQueryable().Where(t => t.PublishedTime.Year == year && t.PublishedTime.Month == month);

            var result = await GetPosts(query, page);

            return new Tuple<DateTime, IPagedList<PostViewModel>>(new DateTime(year, month, 1), result);
        }

        private async Task<IPagedList<PostViewModel>> GetPostsBySearch(string q, int page)
        {
            var query = _postManager.GetQueryable().Where(t => t.Title.Contains(q));

            var result = await GetPosts(query, page);

            return result;
        }

        private async Task<IPagedList<PostViewModel>> GetPosts(int page)
        {
            var query = _postManager.GetQueryable();

            return await GetPosts(query, page);
        }

        private async Task<IPagedList<PostViewModel>> GetPosts(IQueryable<Post> query, int page)
        {
            var pageSize = _basicSettings.PageShowCount;
            if (pageSize <= 1) pageSize = 1;

            var queryResult = await Task.FromResult(query.Where(t => !t.IsDraft).OrderByDescending(t => t.PublishedTime).ToPagedList(page, pageSize));

            var models = new List<PostViewModel>();

            foreach (var item in queryResult)
            {
                var model = item.ToModel();

                await PreparePostViewModel(model, item);

                models.Add(model);
            }

            var result = new PagedList<PostViewModel>(models, queryResult.PageNumber, queryResult.PageSize, queryResult.TotalItems);

            ViewData["pageMeta"] = result;

            return await Task.FromResult(result);
        }

        #endregion

        #region Post

        [Route("post/{slug?}")]
        public async Task<IActionResult> Post(string id, string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return View("NotFound");
            }

            var model = await GetPost(slug);

            if (model == null)
            {
                return View("NotFound");
            }

            await _postManager.IncreaseViewCountAsync(model.Id);

            await SetTitleAsync(model.Title, string.Join(",", model.Tags), model.Description);

            return View(model);
        }

        private async Task<PostViewModel> GetPost(string slug)
        {
            var post = await _postManager.FindBySlugAsync(slug);

            if (post != null)
            {
                var model = post.ToModel();

                await PreparePostViewModel(model, post);

                return model;
            }

            return null;
        }



        #endregion

        #region Page

        [Route("page/{slug?}")]
        public async Task<IActionResult> Page(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return View("NotFound");
            }

            var model = await GetPage(slug);

            if (model == null)
            {
                return View("NotFound");
            }

            await SetTitleAsync(model.Title, model.Keywords, model.Description);

            return View(model);
        }

        private async Task<PageViewModel> GetPage(string slug)
        {
            var page = await _pageManager.FindBySlugAsync(slug);

            if (page == null)
                return null;

            var model = page.ToModel();

            return model;
        }

        #endregion

        #region Archive

        public async Task<IActionResult> Archive()
        {
            var categories = await Task.FromResult(_categoryManager.GetQueryable().ToList());

            var categoryModels = new List<CategoryViewModel>();

            foreach (var category in categories)
            {
                var cmodel = category.ToModel();

                cmodel.Count = await _categoryManager.GetPostCountAsync(category.Id, false);

                var posts = _postManager.GetQueryable().Where(t => !t.IsDraft && t.Categories.Any(c => c.CategoryId == category.Id)).ToList();

                cmodel.Posts = posts.Select(t => t.ToModel()).ToList();

                categoryModels.Add(cmodel);
            }

            var model = new ArchiveViewModel()
            {
                Categories = categoryModels,
            };

            model.NoCategoryPosts = _postManager.GetQueryable().Where(t => !t.IsDraft && t.Categories.Count == 0).ToList().Select(t => t.ToModel()).ToList();

            return View(model);
        }

        #endregion

        #region comments

        [HttpPost]
        public async Task<IActionResult> AddComment(CommentFormViewModel model)
        {
            if (!_commentsSettings.EnableComments)
                return Json(new { result = false, message = "Comment Not Allowed! " }); //ReturnCommentResult(null, new { result = false });

            if (string.IsNullOrEmpty(model.PostId))
                return Json(new { result = false, message = "Error" });  //return ReturnCommentResult(null, new { result = false });

            var post = await _postManager.FindByIdAsync(model.PostId);

            if (post == null)
            {
                return Json(new { result = false, message = "Error" });
            }

            if (ModelState.IsValid)
            {
                var comment = new Comment()
                {
                    Author = model.Author,
                    Content = model.Content,
                    Website = model.Website,
                    Email = model.Email,
                    ParentId = model.ParentId,
                    PostId = post.Id,
                };

                var httpFeatures = HttpContext.Features.Get<IHttpConnectionFeature>();

                comment.IP = httpFeatures.LocalIpAddress?.ToString();

                comment.IsApproved = !_commentsSettings.EnableCommentsModeration;

                if (_commentsSettings.TrustAuthenticatedUsers)
                {
                    bool isTrust = await _commentManager.IsTrustUserAsync(comment.Email);
                    comment.IsApproved = isTrust;
                }

                await _commentManager.CreateAsync(comment);

                await _postManager.IncreaseCommentsCountAsync(post.Id);

                SaveCommentFormUser(new LastCommentFormUserInfo() { Author = model.Author, Email = model.Email, Website = model.Website });

                //  send email
                if (_commentsSettings.SendEmail)
                {
                    Passingwind.Blog.Comment parentComment = null;
                    if (!string.IsNullOrEmpty(model.ParentId))
                    {
                        parentComment = await _commentManager.FindByIdAsync(model.ParentId);
                    }

                    string url = Url.PostCommentLink(post.Slug, Request.Scheme, host: Request.Host.Value, fragment: $"comment-{comment.Id}");
                    // Url.Action("post", values: new { slug }, protocol: Request.Protocol, host: Request.Host.ToString(), fragment: $"comment-{comment.Id}");
                    await _emailSender.SendCommentEmailAsync(_emailSettings, post, url, comment, parentComment);
                }

                return Json(new { result = true, commentId = comment.Id, parentId = comment.ParentId, url = Url.RouteUrl("Comment", new { commentId = comment.Id }) });
                //return ReturnCommentResult(post, new { result = true, comment = comment.ToModel() });
            }

            return Json(new { result = false, message = string.Join(" ", ModelState.Values.SelectMany(t => t.Errors.Select(e => e.ErrorMessage))) });
        }

        [Route("/comment/details/", Name = "Comment")]
        public async Task<IActionResult> Comment(string commentId)
        {
            if (string.IsNullOrEmpty(commentId))
                return NotFound();

            var comment = await _commentManager.FindByIdAsync(commentId);

            if (comment == null)
                return NotFound();

            return View(comment.ToModel());
        }

        private void SaveCommentFormUser(LastCommentFormUserInfo user)
        {
            if (user == null)
                return;

            var jsonString = JsonConvert.SerializeObject(user);

            Response.Cookies.Append(
                "lastCommentForm",
                jsonString,
                new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true
                });
        }

        private IActionResult ReturnCommentResult(Post post, object data)
        {
            if (Request.IsAjax())
                return Json(data);

            if (post == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Post), new { slug = post.Slug });
        }

        #endregion

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        #region Helper

        private async Task SetTitleAsync(string title, string keyworlds = null, string description = null, int page = 0)
        {
            var titleParts = new List<string>();

            if (string.IsNullOrEmpty(title))
            {
                titleParts.Add(_basicSettings.Title);
                titleParts.Add(_basicSettings.Description);
            }
            else
            {
                titleParts.Add(title);
                titleParts.Add(_basicSettings.Title);
            }

            if (page > 1)
                titleParts.Add("Page:" + page);




            if (string.IsNullOrEmpty(description))
                description = _basicSettings.Description;

            //if (string.IsNullOrEmpty(title))
            //{
            //    title = _basicSettings.Title + " | " + _basicSettings.Description;
            //    description = _basicSettings.Description;
            //}

            //if (page > 1)
            //    title = title + " | Page " + page;

            if (string.IsNullOrEmpty(keyworlds))
            {
                var allCategoryName = await Task.FromResult(_categoryManager.GetQueryable().Select(t => t.Name).ToList());
                var allTags = await Task.FromResult(_tagManager.GetQueryable().Select(t => t.Name).ToList());

                keyworlds = string.Join(",", allCategoryName);
            }

            ViewData["Title"] = string.Join(" | ", titleParts.ToArray());
            ViewData["KeyWorlds"] = keyworlds;
            ViewData["Description"] = description;
        }

        private async Task PreparePostViewModel(PostViewModel model, Post entity)
        {
            var categories = await _postManager.GetCategoriesAsync(entity.Id);

            model.Categories = categories.Select(t => t.ToModel()).ToList();

            model.Tags = await _postManager.GetTagsStringListAsync(entity.Id);

        }

        #endregion


        #region Error 

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View("NotFound");
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion


        public IActionResult Test()
        {
            return View();
        }
    }
}
