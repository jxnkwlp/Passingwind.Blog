using System.Collections.Generic;

namespace Passingwind.Blog.Widgets.WidgetComponents
{
	public class DefaultWidgetViewLocationExpander : IWidgetViewLocationExpander
	{
		public IEnumerable<string> ExpandViewLocations(WidgetViewLocationExpanderContext context, IEnumerable<string> locations)
		{
			return locations;
		}
	}
}
