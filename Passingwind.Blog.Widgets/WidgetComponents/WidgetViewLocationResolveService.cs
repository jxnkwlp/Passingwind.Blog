using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets.WidgetComponents
{
	public class WidgetViewLocationResolveService : IWidgetViewLocationResolveService
	{
		private const string DefaultViewName = "Default";
		private const string ViewPathFormat = "/{0}/Views/{1}";  // <widget-name>/Views/<viewname=default.cshtml>

		private readonly ICompositeViewEngine _viewEngine;

		private readonly IWidgetViewLocationExpander _widgetViewLocationExpander;

		public WidgetViewLocationResolveService(ICompositeViewEngine viewEngine, IWidgetViewLocationExpander widgetViewLocationExpander)
		{
			_viewEngine = viewEngine;
			_widgetViewLocationExpander = widgetViewLocationExpander;
		}

		public IEnumerable<string> Search(WidgetComponentDescriptor descriptor)
		{
			var locations = _widgetViewLocationExpander.ExpandViewLocations(new WidgetViewLocationExpanderContext()
			{
				ComponentDescriptor = descriptor,
			}, new string[] { ViewPathFormat });

			return locations;
		}
	}
}
