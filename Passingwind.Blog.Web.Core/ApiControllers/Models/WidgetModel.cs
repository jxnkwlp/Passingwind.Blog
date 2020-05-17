using Passingwind.Blog.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
	public class WidgetListModel
	{
		public string WidgetId { get; set; }
		public string WidgetName { get; set; }
		 
		public string AdminConfigureUrl { get; set; }
	}

	public class WidgetZoneConfigItemModel
	{
		public string Zone { get; set; }

		public IEnumerable<WidgetPositionConfigModel> Widgets { get; set; }
	}
}
