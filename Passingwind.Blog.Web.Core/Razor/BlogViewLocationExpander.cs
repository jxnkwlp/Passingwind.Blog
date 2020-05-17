using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog.Web.Razor
{
	public class BlogViewLocationExpander : IViewLocationExpander
	{
		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			var extensionLocations = new[] {
				  $"/Views/{{0}}.cshtml",
				  $"/Views/Shared/{{0}}.cshtml",
			};

			return extensionLocations.Concat(viewLocations);
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			// NO-OP
		}
	}
}
