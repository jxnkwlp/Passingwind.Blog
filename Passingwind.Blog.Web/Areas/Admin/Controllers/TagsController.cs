using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Pager;
using Passingwind.Blog.Web.Areas.Admin.Models;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class TagsController : AdminControllerBase
    {
        private readonly TagsManager _tagsManager;


        public TagsController(TagsManager tagsManager)
        {
            this._tagsManager = tagsManager;

        }

        public IActionResult List(int page, string q)
        {
            var query = _tagsManager.GetQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.Name.Contains(q));

            var models = query.ToPagedList(page, TableListItem, s => s.Select(t => t.ToModel()).ToList());

            return View(models);
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
                    await _tagsManager.DeleteByNameAsync(item);
                }

                AlertSuccess("已删除。");
            }

            return RedirectToAction(nameof(List));
        }
    }
}
