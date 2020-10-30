using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Data.Domains
{
	public class WidgetDynamicContent : Entity<int>
	{
		public string UserId { get; set; }
		public Guid WidgetId { get; set; }

		public virtual List<WidgetDynamicContentProperty> Properties { get; set; } = new List<WidgetDynamicContentProperty>();
	}
}
