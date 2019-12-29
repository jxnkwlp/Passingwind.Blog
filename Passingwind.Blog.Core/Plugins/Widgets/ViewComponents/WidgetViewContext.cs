using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public class WidgetViewContext
	{
		public HttpContext HttpContext { get; set; }

		public IDictionary<string, object> Arguments { get; set; }

		public WidgetDescriptor Descriptor { get; }

		public WidgetViewContext(HttpContext httpContext, WidgetDescriptor widgetDescriptor, IDictionary<string, object> arguments)
		{
			Arguments = arguments;
			HttpContext = httpContext;
			Descriptor = widgetDescriptor;
		}
	}
}