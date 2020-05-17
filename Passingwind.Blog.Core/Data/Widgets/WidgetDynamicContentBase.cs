using System;

namespace Passingwind.Blog.Data.Widgets
{
	public abstract class WidgetDynamicContentBase : IWidgetDynamicContent
	{
		public virtual int Id { get; set; }
		public virtual string UserId { get; set; }
		public virtual Guid WidgetId { get; set; }
	}
}
