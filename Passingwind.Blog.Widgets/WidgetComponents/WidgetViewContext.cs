using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;

namespace Passingwind.Blog.Widgets
{
	public class WidgetViewContext
	{
		public WidgetConfigurationModel ConfigurationInfo { get; set; }

		public WidgetComponentDescriptor ComponentDescriptor { get; set; }
		public IDictionary<string, object> Arguments { get; set; }

		public ViewContext ViewContext { get; set; }
		public ViewDataDictionary ViewData => ViewContext.ViewData;
		public ITempDataDictionary TempData => ViewContext.TempData;
		public TextWriter Writer => ViewContext.Writer;

		public HtmlEncoder HtmlEncoder { get; set; }


		public WidgetViewContext()
		{
			ComponentDescriptor = new WidgetComponentDescriptor();
			ViewContext = new ViewContext();
		}

		public WidgetViewContext(WidgetComponentDescriptor componentDescriptor, IDictionary<string, object> arguments, ViewContext viewContext, HtmlEncoder htmlEncoder, TextWriter writer)
		{
			ComponentDescriptor = componentDescriptor;
			Arguments = arguments;
			HtmlEncoder = htmlEncoder;

			ViewContext = new ViewContext(
				viewContext,
				viewContext.View,
				new ViewDataDictionary<object>(viewContext.ViewData),
				writer);
		}
	}
}
