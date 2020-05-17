using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Passingwind.Blog.Data.Settings;
using Passingwind.Blog.Services;
using Passingwind.Blog.Web.Factory;
using Passingwind.Blog.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ViewComponents
{
	public class CommentsViewComponent : ViewComponent
	{
		private readonly IPostService _postService;
		private readonly ICommentService _commentService;
		private readonly CommentsSettings _commentsSettings;
		private readonly IPostFactory _postFactory;

		public CommentsViewComponent(IPostService postService, ICommentService commentService, CommentsSettings commentsSettings, IPostFactory postFactory, ICommentFactory commentFactory)
		{
			_postService = postService;
			_commentService = commentService;
			_commentsSettings = commentsSettings;
			_postFactory = postFactory;
		}

		public async Task<IViewComponentResult> InvokeAsync(int postId)
		{
			if (postId <= 0)
			{
				return Content("Not found.");
			}

			var post = await _postService.GetByIdAsync(postId);

			if (post == null)
			{
				return Content("Not found.");
			}

			var postViewModel = _postFactory.ToModel(post, new PostModel());

			postViewModel.EnableComment = postViewModel.EnableComment && _commentsSettings.EnableComments;

			var comments = await _commentService.GetCommentsByPostId(postId, true);

			var commentModels = comments.Select(t => t.ToViewModel(_commentsSettings.CommentNestingEnabled)).ToList();

			var results = commentModels.Format();

			var form = new CommentFormViewModel()
			{
				PostId = post.Id,
				EnableCaptchaCode = _commentsSettings.EnableFormVerificationCode,
			};

			LoadLastCommentFormUser(form);

			return View(new Tuple<PostModel, IList<CommentViewModel>, CommentFormViewModel>(postViewModel, results, form));
		}

		private void LoadLastCommentFormUser(CommentFormViewModel model)
		{
			var cookieValue = Request.Cookies["lastCommentUser"];
			if (!string.IsNullOrEmpty(cookieValue))
			{
				try
				{
					var lastForm = JsonConvert.DeserializeObject<LastCommentFormUserInfo>(cookieValue);

					model.Author = lastForm.Author;
					model.Email = lastForm.Email;
					model.Website = lastForm.Website;
				}
				catch (Exception)
				{
				}
			}
		}


	}
}
