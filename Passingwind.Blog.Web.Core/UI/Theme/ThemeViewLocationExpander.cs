using Microsoft.AspNetCore.Mvc.Razor;
using Passingwind.Blog.Web.Themes;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog.Web.UI.Theme
{
	public class ThemeViewLocationExpander : IViewLocationExpander
	{
		private static readonly string _currentThemeName = null;

		public ThemeViewLocationExpander()
		{
		}

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			if (context.Values.TryGetValue(ThemeConsts.ThemeKey, out var themeName))
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

				var themeViewLocations = viewLocations.Select(t => t.Replace("/Views/", $"/Themes/{themeName}/Views/"));

				viewLocations = themeViewLocations.Concat(viewLocations);
			}

			return viewLocations;
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			if (!string.IsNullOrEmpty(context.AreaName))
				return;

			var themeName = (string)context.ActionContext.HttpContext.Items[ThemeConsts.ThemeKey];
			context.Values[ThemeConsts.ThemeKey] = themeName;

			//if (_currentThemeName != themeName && !string.IsNullOrWhiteSpace(themeName))
			//{
			//	_currentThemeName = themeName;
			//	context.Values[ThemeConsts.ThemeUpdateKey] = DateTime.Now.ToString();
			//}
		}
	}
}
