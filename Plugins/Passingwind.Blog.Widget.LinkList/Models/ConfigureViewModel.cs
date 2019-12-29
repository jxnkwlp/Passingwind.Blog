using System;
using System.Collections.Generic;
using System.Text;

namespace Passingwind.Blog.Widget.LinkList.Models
{
	public class ConfigureViewModel
	{
		public Guid Id { get; set; }

		public IList<LinkModel> Links { get; set; } = new List<LinkModel>();
	}
}
