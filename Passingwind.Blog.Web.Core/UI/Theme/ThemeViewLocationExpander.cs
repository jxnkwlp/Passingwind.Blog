using Microsoft.AspNetCore.Mvc.Razor;
using Passingwind.Blog.Services;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog.Web.UI.Theme
{
	public class ThemeViewLocationExpander : IViewLocationExpander
	{
		private const string ThemeKey = "blog.theme";

		public ThemeViewLocationExpander()
		{
		}

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			if (context.ActionContext.RouteData.Values.TryGetValue(ThemeKey, out object themeName))
			{
				//var themeViewLocations = new List<string> {
				//							 //$"/Themes/{themeName}/Pages/{{0}}.cshtml",
				//							 //$"/Themes/{themeName}/Pages/{{1}}/{{0}}.cshtml",
				//							 $"/Themes/{themeName}/{{0}}.cshtml",
				//							 $"/Themes/{themeName}/{{1}}/{{0}}.cshtml",
				//							 $"/Themes/{themeName}/Shared/{{0}}.cshtml",
				//							 $"/Themes/{themeName}/Views/{{1}}/{{0}}.cshtml",
				//							 $"/Themes/{themeName}/Views/Shared/{{0}}.cshtml",
				//						};

				var themeViewLocations = viewLocations.Select(t => t.Replace("/Views/", $"/Themes/{themeName}/"));

				viewLocations = themeViewLocations.Concat(viewLocations);
			}

			return viewLocations;
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			if (!string.IsNullOrEmpty(context.AreaName))
				return;

			var themeAccessor = context.ActionContext.HttpContext.RequestServices.GetService(typeof(IThemeAccessor)) as IThemeAccessor;
			if (themeAccessor == null)
				return;

			//context.Values[ThemeKey] = themeAccessor.GetCurrentThemeName();
			context.ActionContext.RouteData.Values[ThemeKey] = themeAccessor.GetCurrentThemeName();
		}
	}
}
