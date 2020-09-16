using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets.WidgetComponents
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
			var viewLocationService = ResolveViewLocationService(context);

			if (string.IsNullOrEmpty(ViewName))
				ViewName = DefaultViewName;

			var locations = viewLocationService.Search(context.ComponentDescriptor);

			ViewEngineResult result = null;

			foreach (var viewFormat in locations)
			{
				string viewFullName = string.Format(
						CultureInfo.InvariantCulture,
						viewFormat,
						context.ComponentDescriptor.RootName,
						ViewName
						);

				result = viewEngine.FindView(context.ViewContext, viewFullName + ".cshtml", false);

				if (!result.Success)
				{
					result = viewEngine.GetView(null, viewFullName + ".cshtml", false);
				}

				if (result.Success)
				{
					break;
				}
			}

			IEnumerable<string> searchedLocations = result.SearchedLocations;

			var view = result.EnsureSuccessful(searchedLocations).View;

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

		private static IWidgetViewLocationResolveService ResolveViewLocationService(WidgetViewContext context)
		{
			return context.ViewContext.HttpContext.RequestServices.GetRequiredService<IWidgetViewLocationResolveService>();
		}

	}
}
