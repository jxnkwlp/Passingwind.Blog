using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Widget.MonthList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.MonthList
{
	public class WidgetService : WidgetServiceBase
	{
		private readonly PostManager _postManager;

		public WidgetService(IPluginViewRenderService pluginViewRenderService, PostManager postManager) : base(pluginViewRenderService)
		{
			_postManager = postManager;
		}

		public override Task<object> GetViewDataAsync(PluginDescriptor pluginDescriptor)
		{
			var query = _postManager.GetQueryable().Where(t => !t.IsDraft).Select(t => new DateTime(t.PublishedTime.Year, t.PublishedTime.Month, 1)).ToList();

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

			return Task.FromResult<object>(result);
		}
	}
}
