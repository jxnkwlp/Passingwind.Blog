using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using Passingwind.Blog.Widget.MonthList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.MonthList
{
	public class MonthListWidgetView : WidgetView
	{
		private readonly PostManager _postManager;

		public MonthListWidgetView(PostManager postManager)
		{
			_postManager = postManager;
		}

		public override async Task<IWidgetViewResult> InvokeAsync()
		{
			var list = await _postManager.GetQueryable().Where(t => !t.IsDraft).Select(t => new DateTime(t.PublishedTime.Year, t.PublishedTime.Month, 1)).ToListAsync();

			var months = new SortedDictionary<DateTime, int>();

			var result = new List<MonthListItemViewModel>();

			foreach (var item in list)
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
