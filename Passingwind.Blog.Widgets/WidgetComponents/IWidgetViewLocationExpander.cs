using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets.WidgetComponents
{
	public interface IWidgetViewLocationExpander
	{
		IEnumerable<string> ExpandViewLocations(WidgetViewLocationExpanderContext context, IEnumerable<string> locations);
	}

	public class WidgetViewLocationExpanderContext
	{
		public WidgetComponentDescriptor ComponentDescriptor { get; set; }
	}
}
