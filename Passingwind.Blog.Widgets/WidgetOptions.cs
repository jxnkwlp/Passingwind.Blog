using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Widgets
{
	public class WidgetOptions
	{
		public string Directory { get; set; } = "widgets";

		public Type[] ShardTypes { get; set; } = new[] { typeof(IWidget) };
	}
}
