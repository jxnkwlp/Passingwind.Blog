using Microsoft.AspNetCore.Http;
using Passingwind.Blog.Web.Themes;
using Passingwind.Blog.Widgets.WidgetComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog.Web.UI.Widgets
{
	public class WidgetViewLocationExpander : IWidgetViewLocationExpander
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IThemeAccessor _themeAccessor;

		public WidgetViewLocationExpander(IHttpContextAccessor httpContextAccessor, IThemeAccessor themeAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_themeAccessor = themeAccessor;
		}

		public IEnumerable<string> ExpandViewLocations(WidgetViewLocationExpanderContext context, IEnumerable<string> locations)
		{
			var themeName = AsyncHelper.RunSync(() => _themeAccessor.GetCurrentThemeNameAsync());

			// source path "/{0}/Views/{1}";
			// theme view path = "/{0}/Themes/{2}/{1}";

			if (!string.Equals(themeName, "Default", StringComparison.InvariantCultureIgnoreCase))
			{
				var themeViewLocation = $"/{{0}}/Themes/{themeName}/{{1}}";

				return new string[] { themeViewLocation }.Concat(locations);
			}

			return locations;
		}
	}
}
