using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Areas.Admin.Models;
using Passingwind.Blog.Pager;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class UsersController : AdminControllerBase
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public UsersController(UserManager userManager, RoleManager roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        protected async Task PrepareUserViewModel(UserViewModel model, User user)
        {
            if (user != null)
            {
                model.SelectRoles = (await _userManager.GetRolesAsync(user)).ToArray(); //user.Roles.Select(t => t.RoleId).ToArray();
            }

            model.Roles = _roleManager.GetQueryable().ToList().Select(t => t.ToModel()).ToList();

            //if (user.LockoutEnabled && user.LockoutEnd.HasValue)
            //{
            //    var endTime = user.LockoutEnd.Value.UtcDateTime;

            //    model.Lockouted = DateTimeOffset.UtcNow < endTime;
            //}
            //else
            //    model.Lockouted = false;
        }


        public IActionResult List(int page)
        {
            var query = _userManager.GetQueryable();

            var models = query.ToPagedList(page, TableListItem, s => s.Select(t => t.ToModel()).ToList());

            return View(models);
        }


        public async Task<IActionResult> Create()
        {
            var model = new UserViewModel();

            await PrepareUserViewModel(model, null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                var result = await _userManager.CreateAsync(entity);

                if (result.Succeeded)
                {
                    if (model.Lockouted)
                    {
                        await _userManager.SetLockoutEnabledAsync(entity, model.Lockouted);
                        await _userManager.SetLockoutEndDateAsync(entity, DateTimeOffset.MaxValue);
                    }

                    if (model.SelectRoles != null)
                    {
                        result = await _userManager.AddToRolesAsync(entity, model.SelectRoles);
                    }
                }

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
            var entity = await _userManager.FindByIdAsync(id);
            if (entity == null)
                return NotFound();

            var model = entity.ToModel();

            await PrepareUserViewModel(model, entity);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            var entity = await _userManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                entity = model.ToEntity(entity);

                var result = await _userManager.UpdateAsync(entity);

                if (result.Succeeded)
                {
                    if (model.Lockouted)
                    {
                        await _userManager.SetLockoutEnabledAsync(entity, model.Lockouted);
                        await _userManager.SetLockoutEndDateAsync(entity, DateTimeOffset.MaxValue);
                    }
                    else
                    {
                        await _userManager.SetLockoutEnabledAsync(entity, model.Lockouted);
                        await _userManager.SetLockoutEndDateAsync(entity, DateTimeOffset.MinValue);
                    }

                    await _userManager.RemoveFromAllRolesAsync(entity);

                    if (model.SelectRoles != null)
                    {
                        result = await _userManager.AddToRolesAsync(entity, model.SelectRoles);
                    }
                }

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
                    var user = await _userManager.FindByIdAsync(item);
                    if (user != null)
                    {
                        if (!user.UserName.Equals(Blog.User.DefaultAdministratorUserName, StringComparison.CurrentCultureIgnoreCase))
                            await _userManager.DeleteAsync(user);
                    }
                }

                AlertSuccess("已删除。");
            }

            return RedirectToAction(nameof(List));
        }





        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new UserProfileViewModel()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                UserDescription = user.UserDescription,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                user.DisplayName = model.DisplayName;
                user.UserDescription = model.UserDescription;
                if (model.Email != user.Email)
                {
                    user.EmailConfirmed = false;
                }
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    AlertSuccess("已更新");
                }
                else
                {
                    AlertError(string.Join(",", result.Errors));
                }
            }

            return View(model);
        }
    }
}
