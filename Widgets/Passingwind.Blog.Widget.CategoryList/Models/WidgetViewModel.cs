using System.Collections.Generic;

namespace Passingwind.Blog.Widget.CategoryList.Models
{
	public class WidgetViewModel
	{
		public string Title { get; set; }

		public IEnumerable<CategoryViewModel> List { get; set; }
	}
}
