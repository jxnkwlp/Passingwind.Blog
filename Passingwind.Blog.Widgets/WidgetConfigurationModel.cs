using System;

namespace Passingwind.Blog.Widgets
{
	/// <summary>
	///  The widget configuration data
	/// </summary>
	public class WidgetConfigurationModel
	{
		public Guid Id { get; set; }

		public string WidgetId { get; set; }

		public string WidgetName { get; set; }

		public string Title { get; set; }

		public string Zone { get; set; }

		public int DisplayOrder { get; set; }
	}
}
