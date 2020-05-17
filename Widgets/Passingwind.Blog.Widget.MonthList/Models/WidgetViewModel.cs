using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Widget.MonthList.Models
{
	public class CountModel
	{
		public string Title { get; set; }

		public DateTime DateTime { get; set; }

		public int Count { get; set; }
	}

	public class YearCountModel
	{
		public int Year { get; set; }
		public IEnumerable<CountModel> List { get; set; }
	}

	public class WidgetViewModel
	{
		public string Title { get; set; }
		public YearCountModel[] List { get; set; }
	}
}
