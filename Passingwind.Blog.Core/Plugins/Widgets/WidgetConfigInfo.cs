using System;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetConfigInfo
	{
		public Guid Id { get; set; } = Guid.Empty;

		public int Order { get; set; }

		public string Name { get; set; }
	}
}
