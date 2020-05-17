using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.MonthList.Models;
using Passingwind.Blog.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.MonthList
{
	public class MonthListWidgetView : WidgetComponent
	{
		private readonly IPostService _postService;

		public MonthListWidgetView(IPostService postService)
		{
			_postService = postService;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			var list = await _postService.GetCountsByPublishYearAndMonthAsync();
			 
			var countList = new List<CountModel>();

			//foreach (var item in list)
			//{
			//	months.TryGetValue(item, out int count);
			//	count++;
			//	months[item] = count;
			//}

			countList = list
						.Select(t => new CountModel()
						{
							Title = t.Key.ToString("yyyy/MM"),
							DateTime = t.Key,
							Count = t.Value
						})
						.OrderByDescending(t => t.DateTime)
						.ToList();

			var yearList = countList.GroupBy(t => t.DateTime.Year).Select(t => new YearCountModel()
			{
				Year = t.Key,
				List = t,
			});

			var model = new WidgetViewModel()
			{
				Title = WidgetViewContext.ConfigurationInfo.Title,
				List = yearList.ToArray(),
			};

			return View(model);
		}
	}
}
