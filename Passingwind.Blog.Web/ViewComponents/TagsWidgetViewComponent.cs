using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.ViewComponents
{
    public class TagsWidgetViewComponent : ViewComponent
    {
        private readonly TagsManager _tagsManager;

        public TagsWidgetViewComponent(TagsManager tagsManager)
        {
            this._tagsManager = tagsManager;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tagsList = await Task.FromResult(_tagsManager.GetQueryable());

            var models = new List<TagsViewModel>();

            foreach (var item in tagsList)
            {
                models.Add(new TagsViewModel()
                {
                    Name = item.Name,
                    Count = await _tagsManager.GetPostsCountAsync(item.Id),
                });
            }

            return View(models);
        }
    }
}
