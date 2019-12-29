using System;

namespace Passingwind.Blog.Widget.LinkList.Models
{
	public class LinkModel : IPluginData
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; }
		public string Url { get; set; }
	}
}
