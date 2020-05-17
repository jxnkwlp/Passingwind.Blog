using System.Collections.Generic;

namespace Passingwind.Blog.Widget.LinkList.Models
{ 
	public class WidgetViewModel
	{
		public string Title { get; set; }
		public IEnumerable<LinkModel> List { get; set; }
	}
}
