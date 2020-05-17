using System.Collections.Generic;

namespace Passingwind.Blog.Widget.TagsCloud.Models
{
	public class WidgetViewModel
	{
		public string Title { get; set; }
		public IEnumerable<TagsModel> List { get; set; }
	}
}
