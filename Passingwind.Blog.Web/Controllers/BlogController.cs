using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Settings;
using Passingwind.Blog.EventBus;
using Passingwind.Blog.Json;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Web.Captcha;
using Passingwind.Blog.Web.EventBus;
using Passingwind.Blog.Web.Extensions;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using Passingwind.Blog.Web.Models.Blog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Controllers
{
	[Route("/[action]")]
	public class BlogController : BlogControllerBase
	{
		private readonly ILogger<BlogController> _logger;
		private readonly IPostService _postService;
		private readonly IPageService _pageService;
		private readonly ICategoryService _categoryService;
		private readonly ITagsService _tagsService;
		private readonly ICommentService _commentService;
		private readonly BlogUserManager _blogUserManager;

		private readonly ICaptchaService _captchaService;
		private readonly IIPAddressService _iPAddressService;

		private readonly IPostFactory _postFactory;
		private readonly IPageFactory _pageFactory;
		private readonly ICategoryFactory _categoryFactory;
		private readonly ITagsFactory _tagsFactory;
		private readonly IUserFactory _userFactory;

		private readonly CommentsSettings _commentsSettings;
		private readonly BasicSettings _basicSettings;

		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IEmailSender _emailSender;

		private readonly IJsonSerializer _jsonSerializer;

		private readonly IStringLocalizer<BlogController> _stringLocalizer;
		private readonly ISpamService _spamService;

		private readonly IEventBus _eventBus;

		private readonly IDistributedCache _distributedCache;

		public BlogController(ILogger<BlogController> logger, IPostService postService, IPageService pageService, ICategoryService categoryService, ITagsService tagsService, ICommentService commentService, IPostFactory postFactory, IPageFactory pageFactory, ICategoryFactory categoryFactory, ITagsFactory tagsFactory, BlogUserManager blogUserManager, IUserFactory userFactory, CommentsSettings commentsSettings, ICaptchaService captchaService, IHttpContextAccessor httpContextAccessor, IIPAddressService iPAddressService, IEmailSender emailSender, IJsonSerializer jsonSerializer, IStringLocalizer<BlogController> stringLocalizer, ISpamService spamService, IEventBus eventBus, BasicSettings basicSettings, IDistributedCache distributedCache)
		{
			_logger = logger;
			_postService = postService;
			_pageService = pageService;
			_categoryService = categoryService;
			_tagsService = tagsService;
			_commentService = commentService;
			_postFactory = postFactory;
			_pageFactory = pageFactory;
			_categoryFactory = categoryFactory;
			_tagsFactory = tagsFactory;
			_blogUserManager = blogUserManager;
			_userFactory = userFactory;
			_commentsSettings = commentsSettings;
			_captchaService = captchaService;
			_httpContextAccessor = httpContextAccessor;
			_iPAddressService = iPAddressService;
			_emailSender = emailSender;
			_jsonSerializer = jsonSerializer;
			_stringLocalizer = stringLocalizer;
			_spamService = spamService;
			_eventBus = eventBus;
			_basicSettings = basicSettings;
			_distributedCache = distributedCache;
		}

		#region Get Posts

		public class GetPostListRequestModel
		{
			public int Page { get; set; } = 1;
			public string Q { get; set; }
			public CategoryModel? Category { get; set; }
			public TagsModel? Tags { get; set; }
			public UserModel? Author { get; set; }

			public DateTime? PublishYearMonth { get; set; }
		}


		[NonAction]
		protected async Task<IActionResult> GetPostsAsync(GetPostListRequestModel model)
		{
			var vm = new PostsViewModel();

			var listInputModel = new PostListInputModel()
			{
				Skip = 0,
				Limit = 10,
				IsDraft = false,
				IncludeOptions = new PostIncludeOptions()
				{
					IncludeCategory = true,
					IncludeTags = true,
					IncludeUser = true,
				}
			};

			listInputModel.CategoryId = model.Category?.Id;
			listInputModel.TagsId = model.Tags?.Id;
			listInputModel.UserId = model.Author?.Id;
			listInputModel.PublishedYearMonth = model.PublishYearMonth;

			if (model.Page <= 1) model.Page = 1;
			listInputModel.Skip = (model.Page - 1) * listInputModel.Limit;

			if (!string.IsNullOrWhiteSpace(model.Q))
			{
				listInputModel.SearchTerm = model.Q;
			}

			await SetPageTitleAndMetadataAsync(model);

			var list = await _postService.GetPostsPagedListAsync(listInputModel);

			vm.Posts = list.ReplaceTo((item) => _postFactory.ToModel(item, new Models.PostModel()));

			return View("Posts", vm);
		}

		[Route("/", Name = "home")]
		public async Task<IActionResult> GetPosts([FromQuery] string q = null, [FromQuery] int page = 1)
		{
			return await GetPostsAsync(new GetPostListRequestModel()
			{
				Q = q,
				Page = page,
			});
		}

		[Route("/author/{author}", Name = "author")]
		public async Task<IActionResult> GetListByAuthor([FromRoute] string author, [FromQuery] int page = 1)
		{
			var user = await _blogUserManager.FindByNameAsync(author);
			if (user == null)
				return NotFound();

			var userModel = _userFactory.ToModel(user, new Models.UserModel());

			return await GetPostsAsync(new GetPostListRequestModel()
			{
				Author = userModel,
				Page = page,
			});
		}

		[Route("/category/{category}", Name = "category")]
		public async Task<IActionResult> GetListByCategory([FromRoute] string category, [FromQuery] int page = 1)
		{
			var cat = await _categoryService.GetBySlugAsync(category);
			if (cat == null)
				return NotFound();

			var categoryModel = _categoryFactory.ToModel(cat, new Models.CategoryModel());

			return await GetPostsAsync(new GetPostListRequestModel()
			{
				Page = page,
				Category = categoryModel,
			});
		}

		[Route("/tags/{tags}", Name = "tags")]
		public async Task<IActionResult> GetListByTags([FromRoute] string tags, [FromQuery] int page = 1)
		{
			var t = await _tagsService.GetByNameAsync(tags);
			if (t == null)
				return NotFound();

			var tagsModel = _tagsFactory.ToModel(t, new Models.TagsModel());

			return await GetPostsAsync(new GetPostListRequestModel()
			{
				Page = page,
				Tags = tagsModel,
			});
		}

		[Route("/{year:int}/{month:range(1,12)}", Name = "month")]
		public async Task<IActionResult> GetListByMonth([FromRoute] int year, [FromRoute] int month, [FromQuery] int page = 1)
		{
			return await GetPostsAsync(new GetPostListRequestModel()
			{
				Page = page,
				PublishYearMonth = new DateTime(year, month, 1),
			});
		}

		#endregion

		#region Post

		[Route("/post/{slug}", Name = "post")]
		public async Task<IActionResult> PostAsync([FromRoute] string slug, [FromQuery] int id = 0)
		{
			if (string.IsNullOrEmpty(slug) && id <= 0)
				return NotFound();

			Blog.Data.Domains.Post entity;
			if (id > 0)
			{
				entity = await _postService.GetByIdAsync(id, new PostIncludeOptions()
				{
					IncludeCategory = true,
					IncludeTags = true,
					IncludeUser = true,
				});
			}
			else
			{
				entity = await _postService.FindBySlugAsync(slug, new PostIncludeOptions()
				{
					IncludeCategory = true,
					IncludeTags = true,
					IncludeUser = true,
				});
			}

			if (entity == null)
				return NotFound();

			await _postService.IncreaseViewCountAsync(entity.Id);

			var model = _postFactory.ToModel(entity, new Models.PostModel());

			SetPageTitleAndMetadata(model);

			return View(model);
		}

		#endregion

		#region Page

		[Route("/page/{slug}", Name = "page")]
		public async Task<IActionResult> PageAsync([FromRoute] string slug, [FromQuery] int id = 0)
		{
			if (string.IsNullOrEmpty(slug) && id <= 0)
				return NotFound();

			Page entity;
			if (id > 0)
			{
				entity = await _pageService.GetByIdAsync(id);
			}
			else
			{
				entity = await _pageService.FindBySlugAsync(slug);
			}

			if (entity == null)
				return NotFound();

			var model = _pageFactory.ToModel(entity, new PageModel());

			await SetPageTitleAndMetadataAsync(model);

			return View(model);
		}

		#endregion

		#region Archive

		[Route("/archive", Name = "archive")]
		[ResponseCache(Duration = 60)]
		public async Task<IActionResult> ArchiveAsync()
		{
			var vm = new ArchiveViewModel();

			var categories = await _categoryService.GetListAsync();

			foreach (var item in categories)
			{
				var posts = (await _postService.GetListAsync(t => t.Categories.Any(c => c.CategoryId == item.Id))).OrderByDescending(t => t.PublishedTime);

				var category = _categoryFactory.ToModel(item, new CategoryListItemModel());

				vm.CategoryPosts[category] = posts.Select(p => _postFactory.ToModel(p, new Models.PostModel())).ToArray();
			}

			var noCategoryPosts = (await _postService.GetListAsync(t => t.Categories.Any() == false)).OrderByDescending(t => t.PublishedTime);
			vm.NoCategoryPosts = noCategoryPosts.Select(p => _postFactory.ToModel(p, new Models.PostModel())).ToArray();

			await SetPageTitleAndMetadataAsync();

			return View(vm);
		}

		#endregion

		#region Comment

		[HttpPost("/comment", Name = "SubmitCommend")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SubmitCommentAsync(CommentFormViewModel model)
		{
			if (string.IsNullOrEmpty(Request.Form["_guid_"]))
			{
				return Json(new { result = false, message = "Invalid request." });
			}

			if (!_commentsSettings.EnableComments)
				return Json(new { result = false, message = "Comment Not Allowed! " });

			if (model.PostId <= 0)
				return Json(new { result = false, message = "Error" });

			if (_commentsSettings.EnableFormVerificationCode && !_captchaService.Validate(model.CaptchaId, model.CaptchaCode))
			{
				return Json(new { result = false, message = _stringLocalizer["Invalid Captcha input."].Value });
			}

			var post = await _postService.GetByIdAsync(model.PostId, new PostIncludeOptions() { IncludeUser = true, });

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

				comment.UserId = _httpContextAccessor.HttpContext.User.GetUserId();

				comment.IP = _httpContextAccessor.GetClientIpAddress();
				comment.Country = await _iPAddressService.GetIPLocationAsync(comment.IP);

				comment.IsApproved = !_commentsSettings.EnableCommentsModeration;

				if (_commentsSettings.TrustAuthenticatedUsers)
				{
					comment.IsApproved = await _commentService.IsTrustUserAsync(comment.Email);
				}

				await _commentService.InsertAsync(comment);

				var spamProcessContext = new SpamProcessContext() { Category = "comment", Comment = comment };
				await _spamService.ProcessAsync(spamProcessContext);

				if (spamProcessContext.Passed)
				{
					comment.IsApproved = true;
					comment.IsSpam = false;

					await _commentService.UpdateAsync(comment);
				}

				if (comment.IsApproved)
				{
					await _postService.IncreaseCommentsCountAsync(post.Id);
				}

				SaveCommentFormUser(new LastCommentFormUserInfo() { Author = model.Author, Email = model.Email, Website = model.Website, });

				// event
				var eventData = new CommentApprovedEventData()
				{
					CommentUrl = Url.RouteUrl(RouteNames.Post, new { slug = post.Slug }, Request.Scheme, host: Request.Host.Value, fragment: $"comment-{comment.Id}"),
					Replay = comment,
					Post = post,
					PostAuthor = post.User?.GetDisplayName(),
					PostAuthorEmail = post.User?.Email,
				};

				if (comment.ParentId != null)
				{
					eventData.SourceComment = await _commentService.GetByIdAsync(comment.ParentId.Value);
				}

				_eventBus.Publish(eventData);


				return Json(new { result = true, commentId = comment.GuidId, parentId = comment.ParentId, url = Url.RouteUrl("Comment", new { commentId = comment.GuidId }) });
			}

			return Json(new { result = false, message = string.Join(" ", ModelState.Values.SelectMany(t => t.Errors.Select(e => e.ErrorMessage))) });
		}

		[Route("/comment/details/{commentId}", Name = "Comment")]
		public async Task<IActionResult> Comment(Guid commentId)
		{
			if (commentId == Guid.Empty)
				return NotFound();

			var comment = await _commentService.GetByGuidAsync(commentId);

			if (comment == null)
				return NotFound();

			return View(comment.ToViewModel(_commentsSettings.CommentNestingEnabled));
		}

		private void SaveCommentFormUser(LastCommentFormUserInfo user)
		{
			if (user == null)
				return;

			var jsonString = _jsonSerializer.Serialize(user);

			Response.Cookies.Append(
				"lastCommentUser",
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

		#region Utils

		protected async Task SetPageTitleAndMetadataAsync(GetPostListRequestModel model)
		{
			AppendPageTitle(_basicSettings.Title);

			if (model.Category != null)
			{
				InsertPageTitle(model.Category.Name);
				SetPageDescription($"{model.Category.Description}. {_basicSettings.Description}");
			}
			else if (model.Tags != null)
			{
				InsertPageTitle(model.Tags.Name);
				SetPageDescription($"{_basicSettings.Description}");
			}
			else if (model.Author != null)
			{
				InsertPageTitle(model.Author.DisplayName);

				SetPageDescription($"{model.Author.DisplayName}. {model.Author.Bio}{_basicSettings.Description}");
			}
			else
			{
				AppendPageTitle(_basicSettings.Description);
				SetPageDescription(_basicSettings.Description);
			}

			if (model.Page >= 2)
			{
				InsertPageTitle($"Page {model.Page}");
			}

			var keywords = (string.Join(",", (await _categoryService.GetListAsync()).Select(t => t.Name)));
			SetPageKeywords(keywords);
		}

		protected void SetPageTitleAndMetadata(PostModel model)
		{
			AppendPageTitle(model.Title);
			AppendPageTitle(_basicSettings.Title);
			SetPageDescription(model.Description);
			if (model.Tags != null)
				SetPageKeywords(string.Join(",", model.Tags));
		}

		protected async Task SetPageTitleAndMetadataAsync(PageModel model)
		{
			AppendPageTitle(model.Title);
			AppendPageTitle(_basicSettings.Title);
			SetPageDescription(model.Description);

			var keywords = (string.Join(",", (await _categoryService.GetListAsync()).Select(t => t.Name)));
			SetPageKeywords(keywords);
		}

		protected async Task SetPageTitleAndMetadataAsync()
		{
			AppendPageTitle($"Archive");
			AppendPageTitle(_basicSettings.Title);
			SetPageDescription(_basicSettings.Description);

			var keywords = (string.Join(",", (await _categoryService.GetListAsync()).Select(t => t.Name)));
			SetPageKeywords(keywords);
		}

		#endregion
	}
}
