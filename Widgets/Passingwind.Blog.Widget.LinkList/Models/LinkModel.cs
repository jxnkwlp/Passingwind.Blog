using Passingwind.Blog.Data.Widgets;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Widget.LinkList.Models
{
	public class LinkModel : WidgetDynamicContentBase
	{
		public string Title { get; set; }
		public string Url { get; set; }
	}

	public class LinkContent
	{
		[Required]
		public string Title { get; set; }
		[Required]
		public string Url { get; set; }
	}

}
