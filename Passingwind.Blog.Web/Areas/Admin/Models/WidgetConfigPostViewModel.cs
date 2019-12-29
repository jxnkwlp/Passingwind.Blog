using Passingwind.Blog.Plugins.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
	public class WidgetConfigPostViewModel
	{
		public string Position { get; set; }

		public IList<WidgetConfigItemModel> Config { get; set; }
	}

	public class WidgetConfigItemModel : WidgetConfigInfo
	{
		public int Index { get; set; }
	}
}
