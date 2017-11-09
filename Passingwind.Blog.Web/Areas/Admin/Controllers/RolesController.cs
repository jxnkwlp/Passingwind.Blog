using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Pager;
using Passingwind.Blog.Web.Areas.Admin.Models;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class RolesController : AdminControllerBase
    {
        private readonly RoleManager _roleManager;

        public RolesController(RoleManager pageManager)
        {
            this._roleManager = pageManager;

        }

        public IActionResult List(int page)
        {
            var query = _roleManager.GetQueryable();

            var models = query.ToList().Select(t => t.ToModel()).ToList();

            return View(models);
        }


        public IActionResult Create()
        {
            var model = new RoleViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                var result = await _roleManager.CreateAsync(entity);

                if (!result.Succeeded)
                {
                    AlertError(string.Join(",", result.Errors.Select(t => t.Description)));
                }
                else
                {

                    AlertSuccess("添加成功。");

                    return RedirectToAction(nameof(List));
                }
            }

            return View(model);
        }


        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _roleManager.FindByIdAsync(id);
            if (entity == null)
                return NotFound();

            var model = entity.ToModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            var entity = await _roleManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                entity = model.ToEntity(entity);

                var result = await _roleManager.UpdateAsync(entity);

                if (!result.Succeeded)
                {
                    AlertError(string.Join(",", result.Errors.Select(t => t.Description)));
                }
                else
                {
                    AlertSuccess("修改成功。");

                    return RedirectToAction(nameof(List));
                }
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
                    var role = await _roleManager.FindByIdAsync(item);
                    if (role != null)
                    {
                        if (!role.NormalizedName.Equals(Role.AdministratorName, StringComparison.CurrentCultureIgnoreCase) &&
                            !role.NormalizedName.Equals(Role.EditorName, StringComparison.CurrentCultureIgnoreCase) &&
                            !role.NormalizedName.Equals(Role.Anonymous, StringComparison.CurrentCultureIgnoreCase))
                            await _roleManager.DeleteAsync(role);
                    }
                }

                AlertSuccess("已删除。");
            }

            return RedirectToAction(nameof(List));
        }
    }
}
