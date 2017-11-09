using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Pager;
using Passingwind.Blog.Web.Areas.Admin.Models;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class PostsController : AdminControllerBase
    {
        private readonly PostManager _postManager;
        private readonly CategoryManager _categoryManager;
        private readonly TagsManager _tagsManager;
        private readonly UserManager _userManager;

        public PostsController(PostManager postManager, CategoryManager categoryManager, TagsManager tagsManager, UserManager userManager)
        {
            this._postManager = postManager;
            this._categoryManager = categoryManager;
            this._tagsManager = tagsManager;
            this._userManager = userManager;

        }

        protected void PrepareViewModel(PostViewModel model, Post entity)
        {
            model.AllCategories = _categoryManager.GetQueryable().Select(t => t.ToModel()).ToList();
            model.AllTags = _tagsManager.GetQueryable().Select(t => t.Name).ToArray();

            if (entity != null)
            {
                model.SelectCategories = entity.Categories.Select(t => t.CategoryId).ToList();

                if (entity.Tags != null)
                {
                    //  model.TagsString = string.Join(", ", entity.Tags.Select(t => t.Tags.Name).ToList());
                    model.SelectTags = entity.Tags.Select(t => t.Tags.Name).ToArray();
                }
            }
        }

        protected async Task UpdatePostCategories(PostViewModel model, Post entity)
        {
            var categoryIds = await _postManager.GetPostCategoriesAsync(entity.Id);
            foreach (var item in categoryIds)
            {
                await _postManager.RemoveCategoryAsync(entity, item.CategoryId);
            }

            foreach (var item in model.SelectCategories)
            {
                var category = await _categoryManager.FindByIdAsync(item);
                if (category != null)
                {
                    entity.Categories.Add(new PostCategory() { CategoryId = category.Id, PostId = entity.Id });
                }
            }
        }

        protected async Task UpdatePostTags(PostViewModel model, Post entity)
        {
            var tagsIds = await _postManager.GetTagsAsync(entity.Id);
            foreach (var item in tagsIds)
            {
                await _postManager.RemoveTagsAsync(entity, item.TagsId);
            }

            foreach (var name in model.SelectTags)
            {
                string newName = name.Trim();
                if (string.IsNullOrWhiteSpace(newName))
                    continue;

                // add
                var tagEntity = await _tagsManager.CreateOrUpdateAsync(newName);

                entity.Tags.Add(new PostTags() { PostId = entity.Id, TagsId = tagEntity.Id });
            }

            //if (!string.IsNullOrEmpty(model.TagsString))
            //{
            //    var names = model.TagsString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //    foreach (var name in names)
            //    {
            //        string newName = name.Trim();
            //        if (string.IsNullOrWhiteSpace(newName))
            //            continue;

            //        // add
            //        var tagEntity = await _tagsManager.CreateOrUpdateAsync(newName);

            //        entity.Tags.Add(new PostTags() { PostId = entity.Id, TagsId = tagEntity.Id });
            //    }

            //}
        }

        public IActionResult List(int page, string q, string userId)
        {
            var query = _postManager.GetQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.Title.Contains(q));

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(t => t.UserId == userId);

            var models = query.ToPagedList(page, TableListItem, s => s.Select(t => t.ToModel()).ToList());

            return View(models);
        }


        public IActionResult Create()
        {
            var model = new PostViewModel();

            PrepareViewModel(model, null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                
                // parse slug
                entity.Slug = entity.GetSeName();

                // categories
                await UpdatePostCategories(model, entity);

                // tags
                await UpdatePostTags(model, entity);

                // author 
                var currentUser = await _userManager.GetUserAsync(User);
                entity.UserId = currentUser.Id;


                await _postManager.CreateAsync(entity);

                AlertSuccess("添加成功。");

                return RedirectToAction(nameof(List));
            }

            PrepareViewModel(model, null);

            return View(model);
        }


        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _postManager.FindByIdAsync(id);
            if (entity == null)
                return NotFound();

            var model = entity.ToModel();

            PrepareViewModel(model, entity);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel model)
        {
            var entity = await _postManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                entity = model.ToEntity(entity);

                // parse slug
                entity.Slug = entity.GetSeName();

                // categories
                await UpdatePostCategories(model, entity);

                // tags
                await UpdatePostTags(model, entity);

                // updte
                await _postManager.UpdateAsync(entity);

                AlertSuccess("修改成功。");

                return RedirectToAction(nameof(List));
            }

            PrepareViewModel(model, entity);

            return View(model);
        }

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
                    await _postManager.DeleteByIdAsync(item);
                }

                AlertSuccess("已删除。");
            }

            return RedirectToAction(nameof(List));
        }

        [HttpPost]

        public async Task<IActionResult> Published(params string[] itemId)
        {
            if (itemId == null || itemId.Length == 0)
            {
                AlertError("没有选择。");
            }
            else
            {
                foreach (var item in itemId)
                {
                    await _postManager.UpdateIsPublishAsync(item, true);
                }

                AlertSuccess("已更新。");
            }

            return RedirectToAction(nameof(List));
        }

        [HttpPost]

        public async Task<IActionResult> UnPublished(params string[] itemId)
        {
            if (itemId == null || itemId.Length == 0)
            {
                AlertError("没有选择。");
            }
            else
            {
                foreach (var item in itemId)
                {
                    await _postManager.UpdateIsPublishAsync(item, false);
                }

                AlertSuccess("已更新。");
            }

            return RedirectToAction(nameof(List));
        }

    }
}
