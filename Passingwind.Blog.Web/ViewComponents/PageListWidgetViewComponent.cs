using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.ViewComponents
{
    public class PageListWidgetViewComponent : ViewComponent
    {
        private readonly PageManager _pageManager;

        public PageListWidgetViewComponent(PageManager pageManager)
        {
            this._pageManager = pageManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var query = await Task.FromResult(_pageManager.GetQueryable().Select(t => t.ToModel()).ToList());

            return View(query);
        }
    }
}
