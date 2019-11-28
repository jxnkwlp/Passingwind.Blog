using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetViewEngine : IViewEngine
	{
		public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
		{
			return ViewEngineResult.NotFound(viewName, new string[0]);
		}

		public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
		{ 
			return ViewEngineResult.NotFound(viewPath, new string[0]);
		}
	}
}
