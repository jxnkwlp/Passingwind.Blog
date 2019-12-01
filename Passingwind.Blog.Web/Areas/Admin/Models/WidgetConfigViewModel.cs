using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using System.Collections.Generic;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
	public class WidgetConfigViewModel
	{
		public Dictionary<string, string> Positions { get; set; } = new Dictionary<string, string>();

		public string Position { get; set; }

		public List<PluginDescriptor> AllPlugins { get; set; } = new List<PluginDescriptor>();

		public IList<WidgetConfigInfo> PluginNames { get; set; } = new List<WidgetConfigInfo>();

		public WidgetConfigViewModel()
		{

		}
	}
}
