using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.ViewComponents
{
    public class MonthListWidgetViewComponent : ViewComponent
    {
        private readonly PostManager _postManager;

        public MonthListWidgetViewComponent(PostManager postManager)
        {
            this._postManager = postManager;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var query = await Task.FromResult(_postManager.GetQueryable().Where(t => !t.IsDraft).Select(t => new DateTime(t.PublishedTime.Year, t.PublishedTime.Month, 1)).ToList());

            var months = new SortedDictionary<DateTime, int>();

            var result = new List<MonthListItemViewModel>();

            foreach (var item in query)
            {
                int count = 1;
                months.TryGetValue(item, out count);
                count++;
                months[item] = count;
            }

            result = months.Select(t => new MonthListItemViewModel()
            {
                Title = t.Key.ToString("yyyy/MM"),
                DateTime = t.Key,
                Count = t.Value
            }).OrderByDescending(t => t.DateTime).ToList();

            return View(result);
        }

    }
}
