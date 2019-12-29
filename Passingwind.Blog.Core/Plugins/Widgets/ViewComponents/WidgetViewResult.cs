using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public class WidgetViewResult : IWidgetViewResult
	{ 
		public string ViewPath { get; set; }

		public ViewDataDictionary ViewData { get; set; }
		 
	}
}
