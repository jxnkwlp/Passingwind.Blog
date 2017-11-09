using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.ViewComponents
{
    public class CommentsViewComponent : ViewComponent
    {
        private readonly PostManager _postManager;
        private readonly CommentManager _commentManager;
        private readonly CommentsSettings _commentsSettings;

        public CommentsViewComponent(PostManager postManager, CommentManager commonManager, CommentsSettings commentsSettings)
        {
            this._postManager = postManager;
            this._commentManager = commonManager;
            this._commentsSettings = commentsSettings;

        }

        public async Task<IViewComponentResult> InvokeAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                return Content("Not found.");
            }

            var post = await _postManager.FindByIdAsync(postId);

            if (post == null)
            {
                return Content("Not found.");
            }

            var postViewModel = post.ToModel();

            postViewModel.EnableComment = postViewModel.EnableComment && _commentsSettings.EnableComments;

            var comments = await _commentManager.GetCommentsByPostId(postId, true);

            var commentModels = comments.Select(t => t.ToModel(post.EnableComment && _commentsSettings.EnableComments && _commentsSettings.CommentNestingEnabled)).ToList();

            var results = commentModels.Format();

            var form = new CommentFormViewModel()
            {
                PostId = post.Id
            };

            LoadLastCommentFormUser(form);

            return View(new Tuple<PostViewModel, IList<CommentViewModel>, CommentFormViewModel>(postViewModel, results, form));
        }

        private void LoadLastCommentFormUser(CommentFormViewModel model)
        {
            var cookieValue = Request.Cookies["lastCommentForm"];
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
