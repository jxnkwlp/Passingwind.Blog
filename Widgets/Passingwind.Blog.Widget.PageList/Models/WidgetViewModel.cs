using System.Collections.Generic;

namespace Passingwind.Blog.Widget.PageList.Models
{
	public class WidgetViewModel
	{
		public string Title { get; set; }
		public IEnumerable<PageModel> List { get; set; }
	}
}
