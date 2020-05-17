using System;

namespace Passingwind.Blog.Data.Widgets
{
	public interface IWidgetDynamicContent
	{
		int Id { get; set; }
		string UserId { get; set; }
		Guid WidgetId { get; set; }

	}
}
