using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public class ViewWidgetViewResult : IWidgetComponentViewResult
	{
		private const string DefaultViewName = "Default";
		private const string ViewPathFormat = "/{0}/Views/{1}";

		public string ViewName { get; set; }

		public ViewDataDictionary ViewData { get; set; }

		public ITempDataDictionary TempData { get; set; }

		public IViewEngine ViewEngine { get; set; }

		public async Task ExecuteAsync(WidgetViewContext context)
		{
			var viewEngine = ViewEngine ?? ResolveViewEngine(context);
			var viewContext = context.ViewContext;

			if (string.IsNullOrEmpty(ViewName))
				ViewName = DefaultViewName;

			string viewName = string.Format(
					CultureInfo.InvariantCulture,
					ViewPathFormat,
					context.ComponentDescriptor.RootName,
					ViewName
					);

			ViewEngineResult result = viewEngine.GetView(null, viewName + ".cshtml", false);
			IEnumerable<string> originalLocations = result.SearchedLocations;

			if (!result.Success)
			{
				result = viewEngine.FindView(context.ViewContext, viewName + ".cshtml", false);
			}

			if (!result.Success)
			{
				throw new Exception($"The widget component '{context.ConfigurationInfo.WidgetName }' view not found.");
			}

			var view = result.EnsureSuccessful(originalLocations).View;

			using (view as IDisposable)
			{
				var childViewContext = new ViewContext(
					viewContext,
					view,
					ViewData ?? context.ViewData,
					context.Writer);

				await view.RenderAsync(childViewContext);
			}
		}

		private static IViewEngine ResolveViewEngine(WidgetViewContext context)
		{
			return context.ViewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
		}
	}
}
