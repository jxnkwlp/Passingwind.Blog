using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Guids;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Extensions;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class CommentController : ApiControllerBase
	{
		private readonly ICommentService _commentService;
		private readonly ICommentFactory _commentFactory;
		private readonly BlogUserManager _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IGuidGenerator _guidGenerator;
		private readonly IIPAddressService _iPAddressService;

		public CommentController(ICommentService commentService, ICommentFactory commentFactory, BlogUserManager userManager, IHttpContextAccessor httpContextAccessor, IGuidGenerator guidGenerator, IIPAddressService iPAddressService)
		{
			_commentService = commentService;
			_commentFactory = commentFactory;
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
			_guidGenerator = guidGenerator;
			_iPAddressService = iPAddressService;
		}

		[ApiPermission("comment.list")]
		[HttpGet]
		public async Task<ApiPagedListOutput<CommentModel>> GetListAsync([FromQuery] CommentApiListQueryModel model)
		{
			var list = await _commentService.GetCommentsPagedListAsync(new CommentListInputModel()
			{
				SearchTerm = model.SearchTerm,
				Limit = model.Limit,
				Skip = model.Skip,
				Author = model.Author,
				Email = model.Email,
				PostId = model.PostId,
				Approved = model.Approved,
				Spam = model.Spam,

				IncludeOptions = new CommentListIncludeOptions()
				{
					IncludePosts = true,
				},
			});

			return new ApiPagedListOutput<CommentModel>(list.TotalCount, list.Select(t => _commentFactory.ToModel(t, new CommentModel())).ToList());
		}

		[ApiPermission("comment.setapprove")]
		[HttpPut("approved")]
		public async Task SetIsApproved([FromBody] CommentApprovedUpdateModel model)
		{
			await _commentService.UpdateIsApprovedAsync(model.Id, model.Value);
		}

		[ApiPermission("comment.setspam")]
		[HttpPut("spam")]
		public async Task SetIsIsSpam([FromBody] CommentSpamUpdateModel model)
		{
			await _commentService.UpdateIsSpamAsync(model.Id, model.Value);
		}

		[ApiPermission("comment.replay")]
		[HttpPost("replay")]
		public async Task Replay([FromBody] CommentReplayModel model)
		{
			var comment = await _commentService.GetByIdAsync(model.CommentId);

			if (comment == null)
				throw new Exception("The comment is not found.");

			var ipAddress = _httpContextAccessor.GetClientIpAddress();
			var country = await _iPAddressService.GetIPLocationAsync(ipAddress);
			var cp = _httpContextAccessor.HttpContext.User;
			var replayUser = await _userManager.FindByNameAsync(cp.Identity.Name);

			await _commentService.InsertAsync(new Comment()
			{
				Author = replayUser.GetDisplayName(),
				Category = "post",
				Content = model.Content,
				IP = ipAddress,
				Country = country,
				Email = replayUser.Email,
				GuidId = _guidGenerator.Create(),
				IsApproved = true,
				IsSpam = false,
				ParentId = model.CommentId,
				PostId = comment.PostId,
				UserId = replayUser.Id,
				Website = $"{_httpContextAccessor.HttpContext.Request.Host}",
			});
		}

		[ApiPermission("comment.delete")]
		[HttpDelete]
		public async Task DeleteAsync([FromBody] int[] ids)
		{
			await _commentService.DeleteByAsync(t => ids.Contains(t.Id));
		}
	}
}
