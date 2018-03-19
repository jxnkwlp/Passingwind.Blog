using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Pager;
using Passingwind.Blog.Web.Areas.Admin.Models;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class CommentsController : AdminControllerBase
    {
        private readonly CommentManager _commentManager;
        private readonly UserManager _userManager;
        private readonly PostManager _postManager;

        public CommentsController(CommentManager categoryManager, UserManager userManager, PostManager postManager)
        {
            this._commentManager = categoryManager;
            this._userManager = userManager;
            this._postManager = postManager;
        }

        protected async Task PrepareViewModel(CommentViewModel model, Comment entity)
        {
            if (entity.Post != null)
                model.Post = entity.Post.ToModel();
            else
            {
                var post = await _postManager.FindByIdAsync(model.PostId);
                if (post != null)
                    model.Post = post.ToModel();
            }
        }


        public async Task<IActionResult> List(string postId, int page, string q)
        {
            var query = _commentManager.GetQueryable().Where(t => !t.IsDeleted);

            if (!string.IsNullOrEmpty(postId))
            {
                ViewBag.showPost = false;
                query = query.Where(t => t.PostId == postId);
            }
            else
            {
                ViewBag.showPost = true;
            }

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.Author.Contains(q) || t.Email.Contains(q) || t.Content.Contains(q));

            var models = query.ToPagedList(page, TableListItem, s => s.Select(t => t.ToModel()).ToList());

            foreach (var item in models)
            {
                var entity = await _postManager.FindByIdAsync(item.PostId);
                if (entity != null)
                    item.Post = entity.ToModel();
            }

            return View(models);
        }


        public async Task<IActionResult> Details(string id)
        {
            var entity = await _commentManager.FindByIdAsync(id);
            if (entity == null)
                return NotFound();

            var model = entity.ToModel();

            await PrepareViewModel(model, entity);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(string id, string replyContent)
        {
            var comment = await _commentManager.FindByIdAsync(id);

            if (comment == null)
            {
                return RedirectToAction(nameof(List));
            }

            if (string.IsNullOrEmpty(replyContent))
            {
                return RedirectToAction(nameof(Reply), new { id = id });
            }

            var user = await _userManager.GetUserAsync(User);

            var replyComment = new Comment()
            {
                PostId = comment.PostId,
                Author = user.DisplayName == null ? user.UserName : user.DisplayName,
                Content = replyContent,
                Email = user.Email,
                IsApproved = true,
                ParentId = id,
            };

            await _commentManager.CreateAsync(replyComment);

            AlertSuccess("已回复");

            return RedirectToAction(nameof(List));
        }


        //public IActionResult Create()
        //{
        //    var model = new CommentViewModel();

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(CommentViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = model.ToEntity();

        //        await _commentManager.CreateAsync(entity);

        //        AlertSuccess("添加成功。");

        //        return RedirectToAction(nameof(List));
        //    }

        //    return View(model);
        //}


        //public async Task<IActionResult> Edit(string id)
        //{
        //    var entity = await _commentManager.FindByIdAsync(id);
        //    if (entity == null)
        //        return NotFound();

        //    var model = entity.ToModel();

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(CommentViewModel model)
        //{
        //    var entity = await _commentManager.FindByIdAsync(model.Id);

        //    if (ModelState.IsValid)
        //    {
        //        entity = model.ToEntity(entity);

        //        await _commentManager.UpdateAsync(entity);

        //        AlertSuccess("修改成功。");

        //        return RedirectToAction(nameof(List));
        //    }

        //    return View(model);
        //}

        [HttpPost]

        public async Task<IActionResult> Deletes(params string[] itemId)
        {
            if (itemId == null || itemId.Length == 0)
            {
                AlertError("没有选择。");
            }
            else
            {
                foreach (var item in itemId)
                {
                    var comment = await _commentManager.FindByIdAsync(item);

                    if (comment != null && !string.IsNullOrEmpty(comment.PostId))
                        await _postManager.ReduceCommentsCountAsync(comment.PostId);

                    await _commentManager.SetIsDeletedAsync(item);
                }

                AlertSuccess("已删除。");
            }

            return RedirectToAction(nameof(List));
        }

        [HttpPost]

        public async Task<IActionResult> Approves(params string[] itemId)
        {
            if (itemId == null || itemId.Length == 0)
            {
                AlertError("没有选择。");
            }
            else
            {
                foreach (var item in itemId)
                {
                    await _commentManager.UpdateIsApprovedAsync(item, true);

                    var comment = await _commentManager.FindByIdAsync(item);
                    await _postManager.IncreaseCommentsCountAsync(comment.PostId);
                }

                AlertSuccess("已设置。");
            }

            return RedirectToAction(nameof(List));
        }

        [HttpPost]

        public async Task<IActionResult> UnApproves(params string[] itemId)
        {
            if (itemId == null || itemId.Length == 0)
            {
                AlertError("没有选择。");
            }
            else
            {
                foreach (var item in itemId)
                {
                    await _commentManager.UpdateIsApprovedAsync(item, false);

                    var comment = await _commentManager.FindByIdAsync(item);
                    await _postManager.ReduceCommentsCountAsync(comment.PostId);
                }

                AlertSuccess("已设置。");
            }

            return RedirectToAction(nameof(List));
        }
    }
}
