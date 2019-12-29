using System;

namespace Passingwind.Blog.Widget.CustomHtml.Models
{
	public class CustomHtmlModel : IPluginData
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string Title { get; set; }

		public string Html { get; set; }
	}
}
