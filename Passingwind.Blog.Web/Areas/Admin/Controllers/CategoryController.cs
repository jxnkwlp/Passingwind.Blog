using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Pager;
using Passingwind.Blog.Web.Areas.Admin.Models;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class CategoryController : AdminControllerBase
    {
        private readonly CategoryManager _categoryManager;

        public CategoryController(CategoryManager categoryManager)
        {
            this._categoryManager = categoryManager;

        }

        public IActionResult List(int page, string q)
        {
            var query = _categoryManager.GetQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.Name.Contains(q));

            var models = query.ToPagedList(page, TableListItem, s => s.Select(t => t.ToModel()).ToList());

            return View(models);
        }


        public IActionResult Create()
        {
            var model = new CategoryViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                // parse slug
                entity.Slug = entity.GetSeName();

                await _categoryManager.CreateAsync(entity);

                AlertSuccess("添加成功。");

                return RedirectToAction(nameof(List));
            }

            return View(model);
        }


        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _categoryManager.FindByIdAsync(id);
            if (entity == null)
                return NotFound();

            var model = entity.ToModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            var entity = await _categoryManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                entity = model.ToEntity(entity);

                // parse slug
                entity.Slug = entity.GetSeName();

                await _categoryManager.UpdateAsync(entity);

                AlertSuccess("修改成功。");

                return RedirectToAction(nameof(List));
            }

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
                    await _categoryManager.DeleteByIdAsync(item);
                }

                AlertSuccess("已删除。");
            }

            return RedirectToAction(nameof(List));
        }

    }
}
