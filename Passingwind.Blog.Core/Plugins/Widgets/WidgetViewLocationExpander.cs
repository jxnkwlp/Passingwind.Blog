using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetViewLocationExpander : IViewLocationExpander
	{
		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{

			return viewLocations;
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
		}
	}
}
